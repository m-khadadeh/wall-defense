using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "ScarcityBasedActionCost", menuName = "Scriptable Objects/AI/GOAP/Cost/Scarcity Based Action Cost")]
  public class ScarcityBasedActionCost : ActionCost
  {
    [SerializeField] private Entry[] _resources;

    public override void Initialize()
    {
      UpdateCost();
    }

    public override void UpdateCost()
    {
      Cost = 0;
      foreach (var resource in _resources)
      {
        Cost += resource.GetScarcityCost();
      }
    }

    [Serializable]
    public class Entry
    {
      [SerializeField] private WorldState _resource;
      [SerializeField] private int _amountToUse;
      public int GetScarcityCost()
      {
        return (int)(5.0f * ((float)_amountToUse / (float)_resource.StateValue));
      }
    }
  }
}
