using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense.AI
{
  public abstract class Action : ScriptableObject
  {
    public abstract ActionCost GetCost();
    public abstract List<ActionCondition> GetConditions();
    public abstract void GetYields();
    public virtual bool CheckConditions()
    {
      foreach (var condition in GetConditions())
      {
        if (!condition.CheckCondition()) return false;
      }
      return true;
    }
    public abstract void ApplyActionState();
    public abstract void UndoActionState();
  }
}
