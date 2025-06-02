using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using WallDefense.AI;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ColonyData", menuName = "Scriptable Objects/ColonyData")]
  public class ColonyData : ScriptableObject
  {
    [field: SerializeField] public InventoryData Inventory { get; private set; }
    [field: SerializeField] public Wall Wall { get; private set; }
    [SerializeField] private AI.SettlementAI _aiController;
    [SerializeField] private GhoulDiscernmentChalkboard _ghoulBoard;
    List<Action<List<ItemType>>> _onGiftReceived;

    public void Initialize()
    {
      _onGiftReceived = new List<Action<List<ItemType>>>();
      Wall.InitializeWalls();
      if (_aiController != null)
      {
        _aiController.Initialize(this, _ghoulBoard);
      }
    }
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
    public void Subscribe(Action<List<ItemType>> subscriber)
    {
      if (!_onGiftReceived.Contains(subscriber))
      {
        _onGiftReceived.Add(subscriber);
      }
    }

    public int GetAIModifiedActionCost(int baseCost)
    {
      return baseCost;
    }
  }
}
