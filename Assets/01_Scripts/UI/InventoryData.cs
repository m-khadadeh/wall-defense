using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "InventoryData", menuName = "Scriptable Objects/InventoryData")]
  public class InventoryData : ScriptableObject
  {
    [field: SerializeField] public List<ItemAmountEntry> Items { get; private set; }

    [Serializable]
    public class ItemAmountEntry
    {
      [field: SerializeField] public ItemType Type { get; private set; }
      [field: SerializeField] public int Amount { get; private set; }
    }
  }
}
