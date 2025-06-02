using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense.AI
{
  [Serializable]
  public class ActionStateResult
  {
    public enum StateChange { Add, Subtract, Set }
    [SerializeField] private WorldState _state;
    [SerializeField] private StateChange _operator;
    [SerializeField] private int _value;
    private Stack<int> _prevValues;
    public void Initialize()
    {
      ClearStack();
    }
    public void ClearStack()
    {
      _prevValues = new Stack<int>();
    }
    public void ApplyResult()
    {
      _prevValues.Push(_state.StateValue);
      if (_operator != StateChange.Set)
      {
        _state.StateValue += (_operator == StateChange.Add ? 1 : -1) * _value;
      }
      else
      {
        _state.StateValue = _value;
      }
    }

    public void UndoResult()
    {
      _state.StateValue = _prevValues.Pop();
    }
  }
}
