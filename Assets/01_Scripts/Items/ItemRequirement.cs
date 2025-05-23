using System;
using UnityEngine;

namespace WallDefense
{
  [Serializable]
  public class ItemRequirement
  {
    [field: SerializeField] public ItemType RequirementType { get; private set; }
    [field: SerializeField] public Vector2Int MinimumMaximumAmount { get; private set; }

    private int _currentCount;

    public void Add(int count = 1)
    {
      _currentCount += count;
    }

    public void Remove(int count = 1)
    {
      _currentCount -= 1;
    }
    public bool IsFulfilled => _currentCount >= MinimumMaximumAmount.x;

    public void Initialize()
    {
      _currentCount = 0;
    }
  }
}
