using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "Goal", menuName = "Scriptable Objects/AI/GOAP/Goal")]
  public class Goal : ScriptableObject
  {
    [SerializeField] private List<ConditionSet> _goalConditions; // Inner sets use AND. Outer sets user OR.
    [SerializeField] private Priority _priority;
    [SerializeField] private ActionCost _maxAllowedCost;
    [field: SerializeField] public bool ProtectionGoal { get; private set; }
    public int Priority => _priority.PriorityValue;
    public int Cost => _maxAllowedCost.Cost;
    public List<ConditionSet> Conditions => _goalConditions;

    public void Initialize()
    {
      _maxAllowedCost.Initialize();
      _priority.Initialize();
    }

    public void UpdateCost() => _maxAllowedCost.UpdateCost();
    public void UpdatePriority() => _priority.UpdatePriority();

    public bool IsGoalMet()
    {
      foreach (var conditionSet in _goalConditions)
      {
        if (CheckInnerSetCondition(conditionSet.Conditions)) return true;
      }
      return false;
    }

    private bool CheckInnerSetCondition(List<ActionCondition> innerSet)
    {
      foreach (var condition in innerSet)
      {
        if (!condition.CheckCondition()) return false;
      }
      return true;
    }

    public List<ActionCondition> GetEasiestConditionList()
    {
      List<ActionCondition> result = null;
      int smallestSet = int.MaxValue;
      var unfulfilledConditions = GetUnfulfilledConditions();
      foreach (var set in unfulfilledConditions)
      {
        if (set.Count < smallestSet)
        {
          smallestSet = set.Count;
          result = set;
        }
      }
      return result;
    }

    public List<List<ActionCondition>> GetUnfulfilledConditions()
    {
      List<List<ActionCondition>> unfulfilledConditions = new List<List<ActionCondition>>();
      foreach (var conditionSet in _goalConditions)
      {
        var remainingConditions = new List<ActionCondition>();
        foreach (var condition in conditionSet.Conditions)
        {
          if (!condition.CheckCondition())
          {
            remainingConditions.Add(condition);
          }
        }
        unfulfilledConditions.Add(remainingConditions);
      }
      return unfulfilledConditions;
    }

    public int GetShortestDistanceFromStates()
    {
      int cost = int.MaxValue;
      foreach (var set in _goalConditions)
      {
        int currentCost = 0;
        foreach (var condition in set.Conditions)
        {
          currentCost += Mathf.Abs(condition.NeededToFulfill());
        }
        if (currentCost < cost)
          cost = currentCost;
      }
      return cost;
    }
    public int GetShortestDistanceFromStates(Dictionary<WorldState, int> states)
    {
      int cost = int.MaxValue;
      foreach (var set in _goalConditions)
      {
        int currentCost = 0;
        foreach (var condition in set.Conditions)
        {
          currentCost += Mathf.Abs(condition.NeededToFulfill(states[condition.State]));
        }
        if (currentCost < cost)
          cost = currentCost;
      }
      return cost;
    }

    public List<Dictionary<WorldState, int>> StatesToHit()
    {
      var result = new List<Dictionary<WorldState, int>>();
      foreach (var conditionSet in _goalConditions)
      {
        var newState = new Dictionary<WorldState, int>();
        foreach (var condition in conditionSet.Conditions)
        {
          newState[condition.State] = condition.State.StateValue + condition.NeededToFulfill();
        }
        result.Add(newState);
      }
      return result;
    }

    public int CompareTo(Goal otherGoal)
    {
      if (otherGoal == null)
        return 1;
      else
        return Priority.CompareTo(otherGoal.Priority);
    }

    [Serializable]
    public class ConditionSet
    {
      [field: SerializeField] public List<ActionCondition> Conditions { get; private set; }
    }
  }
}
