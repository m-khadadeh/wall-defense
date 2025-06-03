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
    public int Priority => _priority.PriorityValue;
    public int Cost => _maxAllowedCost.Cost;

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
      }
      return unfulfilledConditions;
    }

    [Serializable]
    public class ConditionSet
    {
      [field: SerializeField] public List<ActionCondition> Conditions { get; private set; }
    }
  }
}
