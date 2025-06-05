using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "Planner", menuName = "Scriptable Objects/AI/GOAP/Planner")]
  public class Planner : ScriptableObject
  {
    [SerializeField] private List<Sensor> _sensors;
    [SerializeField] private List<Goal> _goals;
    [SerializeField] private List<HourCountdownActionCost> _countdownCosts;
    [SerializeField] private List<GhoulTimeBasedPriority> _hourPriorities;
    [SerializeField] private List<ActionChoice> _actionPool;
    [SerializeField] private List<ActionYield> _initializableYields;
    [SerializeField] private List<ActionCost> _costs;
    [SerializeField] private List<WorldState> _taskLocks;
    [SerializeField] private List<WorldState> _allStates;
    [SerializeField] private List<ResourceTally> _tallies;
    [SerializeField] private int _maxRecursionLevels;
    [SerializeField] private List<Action> _collectionActions;
    [SerializeField] private int _sidesForRandomCollectionAction;
    private HashSet<WorldState> _locks;
    private Dictionary<WorldState, ResourceTally> _tallyByState;
    private Dictionary<Goal, List<ActionChoice>> _planPerGoal;
    private Dictionary<WorldState, int> _stateBoard;
    private HashSet<Action> _freeActionsToTakeThisHour;
    private SettlementAI _aiType;
    private System.Random _rng;

    public void Initialize(SettlementAI aiType)
    {
      _rng = new System.Random();
      aiType = _aiType;
      _planPerGoal = new Dictionary<Goal, List<ActionChoice>>();
      _locks = new HashSet<WorldState>();
      foreach (var goal in _goals)
      {
        goal.Initialize();
        _planPerGoal[goal] = new List<ActionChoice>();
      }
      CheckSensors();
      foreach (var actionYield in _initializableYields)
      {
        actionYield.Initialize();
      }
      foreach (var cost in _costs)
      {
        cost.Initialize();
      }
      foreach (var taskLock in _taskLocks)
      {
        taskLock.StateValue = 0;
        _locks.Add(taskLock);
      }
      _stateBoard = _allStates.ToDictionary(e => e, e => e.StateValue);
      _tallyByState = _tallies.ToDictionary(e => e.ResourceState, e => e);
      _aiType.SparableAmounts = _tallies.ToDictionary(e => e.DialogueKey, e => e);
    }

    private void CheckSensors()
    {
      foreach (var sensor in _sensors)
      {
        sensor.UpdateState();
      }
    }

    public void OnAfterHour(int hour)
    {
      foreach (var sensor in _sensors)
      {
        sensor.UpdateState();
      }
      foreach (var priority in _hourPriorities)
      {
        priority.OnHour(hour);
      }
      foreach (var cost in _countdownCosts)
      {
        cost.OnHour(hour);
      }
      foreach (var goal in _goals)
      {
        goal.UpdateCost();
        goal.UpdatePriority();
      }
      foreach (var cost in _costs)
      {
        cost.UpdateCost();
      }

      _freeActionsToTakeThisHour = new HashSet<Action>();

      // Collect + wait for what you can
      foreach (var action in _actionPool)
      {
        if (action.WaitAction.CheckConditions()) _freeActionsToTakeThisHour.Add(action.WaitAction);
        else if (action.CollectAction.CheckConditions()) _freeActionsToTakeThisHour.Add(action.CollectAction);
      }

      foreach (var action in _freeActionsToTakeThisHour)
      {
        Debug.Log($"Collecting Free Action {action}");
        action.GetYields();
      }

      foreach (var tally in _tallies)
      {
        tally.SpareableAmount = tally.ResourceState.StateValue;
      }

      // Plan new allocations
      PlanCourseOfAction();

      int collectionAllocations = 0;

      // Allocate new actions
      foreach (var goal in _goals)
      {
        foreach (var choice in _planPerGoal[goal])
        {
          CheckSensors();
          if (choice.StartAction.CheckConditions())
          {
            Debug.Log($"Allocating for {choice.StartAction}");
            if (_collectionActions.Contains(choice.StartAction))
            {
              collectionAllocations++;
            }
            choice.StartAction.GetYields();
          }
        }
      }

      // Random collection based action in order to keep resources up
      if (collectionAllocations == 0 && UnityEngine.Random.Range(0, _sidesForRandomCollectionAction + hour) <= 1)
      {
        _collectionActions.OrderBy(_ => _rng.Next());
        foreach (var action in _collectionActions)
        {
          CheckSensors();
          if (action.CheckConditions())
          {
            action.GetYields();
            break;
          }
        }
      }

      // Calculate SpareableAmounts
        foreach (var sensor in _sensors)
        {
          sensor.UpdateState();
        }
      foreach (var goal in _goals)
      {
        var conditionList = goal.GetEasiestConditionList();
        foreach (var condition in conditionList)
        {
          if (_tallyByState.ContainsKey(condition.State))
          {
            int amount = condition.SpareableAmount();
            if (amount < _tallyByState[condition.State].SpareableAmount)
            {
              _tallyByState[condition.State].SpareableAmount = amount;
            }
          }
        }
      }

    }
    private Goal _currentGoal;

    public void PlanCourseOfAction()
    {
      // Reorder Goals
      _goals.Sort((Goal a, Goal b) => -1 * a.CompareTo(b));
      foreach (var goal in _goals)
      {
        _currentGoal = goal;
        _planPerGoal[goal] = new List<ActionChoice>();
        if (goal.Priority <= 0)
        {
          continue;
        }

        if (goal.IsGoalMet())
          continue;

        List<ActionCondition> conditions = new List<ActionCondition>(goal.GetEasiestConditionList());
        var remainingConditions = GetRemainingConditions(conditions);
        var choices = DoableChoices(remainingConditions, 0, _maxRecursionLevels);
        // Order them based on priority
        var list = new List<(ActionChoice, int)>(choices.Select(e => (e.Key, e.Key.Hours)));
        list.Sort((x, y) => y.Item2.CompareTo(x.Item2));
        _planPerGoal[goal] = list.Select(e => e.Item1).ToList();
        // foreach (var action in _planPerGoal[goal])
        // {
        //   // Debug.Log($"Attempting to {action.StartAction} for {goal}");
        // }
      }
    }

    private Dictionary<ActionChoice, int> DoableChoices(List<ActionCondition> conditions, int level, int maxLevels)
    {
      Dictionary<ActionChoice, List<ActionCondition>> choices = new Dictionary<ActionChoice, List<ActionCondition>>();
      foreach (var condition in conditions)
      {
        if (_locks.Contains(condition.State))
        {
          continue;
        }
        int startingAmount = condition.NeededToFulfill();
        ActionChoice bestChoice = null;
        int difference = Mathf.Abs(startingAmount);
        foreach (var action in _actionPool)
        {
          if (action.TaskLock.StateValue != 0)
            continue;
          action.CollectAction.ApplyActionState();
          int newAmount = condition.NeededToFulfill();
          action.CollectAction.UndoActionState();
          if ((Mathf.Abs(newAmount) < difference && !action.ProtectionTask && !_currentGoal.ProtectionGoal) || (Mathf.Abs(newAmount) == 0 && action.ProtectionTask && _currentGoal.ProtectionGoal))
          {
            bestChoice = action;
            difference = Mathf.Abs(newAmount);
          }
        }
        if (bestChoice != null)
        {
          //Debug.Log($"Needed {startingAmount} {condition.State} to fulfill {_currentGoal}. {bestChoice.CollectAction} brings that to {difference}");
          choices[bestChoice] = bestChoice.StartAction.GetConditions();
        }
      }
      Dictionary<ActionChoice, int> newChoices = new Dictionary<ActionChoice, int>();
      foreach (var choice in choices)
      {
        var remainingConditions = GetRemainingConditions(choice.Value);
        if (remainingConditions.Count > 0)
        {
          // Recurse
          if (level + 1 <= _maxRecursionLevels)
          {
            var levelDownChoices = DoableChoices(remainingConditions, level + 1, _maxRecursionLevels);
            foreach (var levelDownChoice in levelDownChoices)
            {
              newChoices[levelDownChoice.Key] = levelDownChoice.Value;
            }
          }
        }
        else
        {
          newChoices[choice.Key] = level;
        }
      }
      return newChoices;
    }

    private List<ActionCondition> GetRemainingConditions(List<ActionCondition> actionConditions)
    {
      List<ActionCondition> conditions = new List<ActionCondition>();
      foreach (var condition in actionConditions)
      {
        if (!condition.CheckCondition())
          conditions.Add(condition);
      }
      return conditions;
    }

    [Serializable]
    public class ActionChoice
    {
      [field: SerializeField] public Action StartAction { get; private set; }
      [field: SerializeField] public Action WaitAction { get; private set; }
      [field: SerializeField] public Action CollectAction { get; private set; }
      [field: SerializeField] public WorldState TaskLock { get; private set; }
      [field: SerializeField] public int Hours { get; private set; }
      [field: SerializeField] public bool ProtectionTask { get; set; }
    }

    [Serializable]
    public class ResourceTally
    {
      [field: SerializeField] public WorldState ResourceState { get; private set; }
      [field: SerializeField] public string DialogueKey { get; private set; }
      public int SpareableAmount { get; set; }
    }
  }
}
