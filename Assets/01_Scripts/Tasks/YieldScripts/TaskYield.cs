using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public abstract class TaskYield : ScriptableObject
  {
    public abstract void GetYield(ColonyData colony, string choice, List<ItemType> consumed);
  }
}
