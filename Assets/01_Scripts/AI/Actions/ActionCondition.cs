using System;
using UnityEngine;

namespace WallDefense.AI
{
  [Serializable]
  public class ActionCondition
  {
    public enum Comparator { Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual }
    [SerializeField] private WorldState _state;
    [SerializeField] private Comparator _conditionComparator;
    [SerializeField] private int _conditionValue;

    public bool CheckCondition()
    {
      switch (_conditionComparator)
      {
        case Comparator.Equal:
          return _state.StateValue == _conditionValue;
        case Comparator.NotEqual:
          return _state.StateValue != _conditionValue;
        case Comparator.LessThan:
          return _state.StateValue < _conditionValue;
        case Comparator.LessThanOrEqual:
          return _state.StateValue <= _conditionValue;
        case Comparator.GreaterThan:
          return _state.StateValue > _conditionValue;
        case Comparator.GreaterThanOrEqual:
          return _state.StateValue >= _conditionValue;
        default:
          throw new Exception("Invalid Comparator");
      }
    }
  }
}
