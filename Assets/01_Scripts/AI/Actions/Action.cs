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
    public abstract List<ActionYield> Yields { get; }
    public virtual bool CheckConditions()
    {
      foreach (var condition in GetConditions())
      {
        if (!condition.CheckCondition()) return false;
      }
      return true;
    }
    public virtual List<ActionCondition> GetUnfulfilledConditions()
    {
      List<ActionCondition> unfulfilledConditions = new List<ActionCondition>();
      var conditions = GetConditions();
      foreach (var condition in conditions)
      {
        if (!condition.CheckCondition())
        {
          unfulfilledConditions.Add(condition);
        }
      }
      return unfulfilledConditions;
    }
    public abstract void ApplyActionState();
    public abstract void UndoActionState();
  }
}
