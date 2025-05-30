using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ItemTaskYield", menuName = "Scriptable Objects/Tasks/Yields/Item Task Yield")]
  public class ItemTaskYield : TaskYield
  {
    [SerializeField] private ItemType _type;
    [SerializeField] private Vector2Int _minMaxValues;
    public override void GetYield(ColonyData colony, string choice, List<ItemType> consumed)
    {
      int amount = (_minMaxValues.x != _minMaxValues.y) ? Random.Range(_minMaxValues.x, _minMaxValues.y + 1) : _minMaxValues.x;
      colony.Inventory.AddItem(_type, amount);
    }
  }
}
