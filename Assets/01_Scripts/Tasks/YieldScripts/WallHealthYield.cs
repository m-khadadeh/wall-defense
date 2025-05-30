using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "WallHealthYield", menuName = "Scriptable Objects/Tasks/Yields/Wall Health Yield")]
  public class WallHealthYield : WallSegmentYield
  {
    [SerializeField] private int _healthAmount;
    public override void GetYield(ColonyData colony, string choice, List<ItemType> consumed)
    {
      InitializeDictionaryIfNot();
      colony.Wall.RepairWall(_dictionary[choice], _healthAmount);
    }    
  }
}
