using System;
using UnityEngine;

namespace WallDefense.AI
{
  [Serializable]
  public class ActionStateResult
  {
    public enum StateChange { Add, Subtract }
    [SerializeField] private WorldState _state;
    [SerializeField] private StateChange _operator;
    [SerializeField] private int _value;

    public void ApplyResult()
    {
      _state.StateValue += (_operator == StateChange.Add ? 1 : -1) * _value;
    }

    public void UndoResult()
    {
      _state.StateValue += (_operator == StateChange.Add ? -1 : 1) * _value;
    }
  }
}
