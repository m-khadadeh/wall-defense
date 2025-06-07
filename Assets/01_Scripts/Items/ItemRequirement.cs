using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [Serializable]
  public class ItemRequirement
  {
    [field: SerializeField] public ItemType RequirementType { get; private set; }
    [field: SerializeField] public Vector2Int MinimumMaximumAmount { get; private set; }
    [field: SerializeField] public bool IsConsumed { get; private set; }

    private List<ItemType> _currentItems;
    private Action _onReset;
    public List<ItemType> CurrentItems => _currentItems;
    public int Count => _currentItems.Count;

    public void Add(ItemType item)
    {
      _currentItems.Add(item);
    }

    public void Remove(ItemType item)
    {
      _currentItems.Remove(item);
    }
    public bool IsFulfilled => Count >= MinimumMaximumAmount.x;

    public void Reset()
    {
      _currentItems = new List<ItemType>();
      _onReset?.Invoke();
    }

    public void InitializeUI(Action resetCallback)
    {
      _currentItems = new List<ItemType>();
      _onReset = resetCallback;
    }

    public void Unbind()
    {
      _onReset = null;
    }
  }
}
