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
    public WorldState State => _state;

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
    public int NeededToFulfill(int stateValue)
    {
      switch (_conditionComparator)
      {
        case Comparator.Equal:
          return _conditionValue - stateValue;
        case Comparator.NotEqual:
          return _conditionValue == stateValue ? 1 : 0;
        case Comparator.LessThan:
          return stateValue < _conditionValue ? 0 : _conditionValue - stateValue - 1;
        case Comparator.LessThanOrEqual:
          return stateValue <= _conditionValue ? 0 : _conditionValue - stateValue;
        case Comparator.GreaterThan:
          return stateValue > _conditionValue ? 0 : _conditionValue - stateValue + 1;
        case Comparator.GreaterThanOrEqual:
          return stateValue >= _conditionValue ? 0 : _conditionValue - stateValue;
        default:
          throw new Exception("Invalid Comparator");
      }
    }
    public int NeededToFulfill() => NeededToFulfill(_state.StateValue);

    public int SpareableAmount()
    {
      if (_conditionComparator == Comparator.Equal || _conditionComparator == Comparator.GreaterThan || _conditionComparator == Comparator.GreaterThanOrEqual)
      {
        return _state.StateValue - _conditionValue - (_conditionComparator == Comparator.GreaterThan ? 1 : 0); 
      }
      else
      {
        return 0;
      }
    }
  }
}
