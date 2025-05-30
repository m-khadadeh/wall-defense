using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ColonyData", menuName = "Scriptable Objects/ColonyData")]
  public class ColonyData : ScriptableObject
  {
    [field: SerializeField] public InventoryData Inventory { get; private set; }
    [field: SerializeField] public Wall Wall { get; private set; }
    List<Action<List<ItemType>>> _onGiftReceived;
    public void ReceiveGift(List<ItemType> items)
    {
      foreach (var item in items)
      {
        Debug.Log($"{name} received {item}");
        Inventory.AddItem(item, 1);
      }
      foreach (var subscriber in _onGiftReceived)
      {
        subscriber.Invoke(items);
      }
    }
  }
}
