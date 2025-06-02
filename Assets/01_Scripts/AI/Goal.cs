using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "Goal", menuName = "Scriptable Objects/AI/GOAP/Goal")]
  public class Goal : ScriptableObject
  {
    [SerializeField] private ActionCondition[] _goalConditions;
    [SerializeField] private Priority _priority;
    [SerializeField] private ActionCost _maxAllowedCost;
    public int Priority => _priority.PriorityValue;

    public void UpdatePriority()
    {
      _priority.UpdatePriority();
    }

    public List<ActionCondition> GetUnfulfilledConditions()
    {
      return _goalConditions.Where(condition => !condition.CheckCondition()).ToList();
    }
  }
}
