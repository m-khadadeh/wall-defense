using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "InventoryData", menuName = "Scriptable Objects/InventoryData")]
  public class InventoryData : ScriptableObject
  {
    [SerializeField] private List<ItemAmountEntry> _startingItems;
    private Dictionary<ItemType, int> _currentItems;
    private Action<ItemType, int> _onAdd;
    private Action<ItemType, int> _onRemove;
    public Dictionary<ItemType, int> CurrentItems => _currentItems;
    public void Initialize(Action<ItemType, int> addCallback, Action<ItemType, int> removeCallback)
    {
      _currentItems = new Dictionary<ItemType, int>();
      foreach (var entry in _startingItems)
      {
        _currentItems.Add(entry.Type, entry.Amount);
      }
      _onAdd = addCallback;
      _onRemove = removeCallback;
    }

    public void AddItem(ItemType type, int amount)
    {
      if (!_currentItems.ContainsKey(type))
      {
        _currentItems[type] = 0;
      }
      _currentItems[type] += amount;
      _onAdd.Invoke(type, amount);
    }
    public void RemoveItem(ItemType type, int amount)
    {
      _currentItems[type] -= amount;
      _onRemove.Invoke(type, amount);
    }

    public void DropItemIn(ItemType type)
    {
      _currentItems[type]++;
    }

    public void DragItemOut(ItemType type)
    {
      _currentItems[type]--;
    }

    [Serializable]
    public class ItemAmountEntry
    {
      [field: SerializeField] public ItemType Type { get; private set; }
      [field: SerializeField] public int Amount { get; private set; }
    }
  }
}
