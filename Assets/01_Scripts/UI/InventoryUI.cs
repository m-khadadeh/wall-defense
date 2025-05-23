using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public class InventoryUI : MonoBehaviour
  {
    [SerializeField] private InventoryData _data;
    [SerializeField] private DroppableZone _itemZone;
    [SerializeField] private ItemPrefabEntry[] _itemPrefabEntries;

    private Dictionary<ItemType, GameObject> _itemPrefabs;
    void Start()
    {
      // Set up dictionaries
      _itemPrefabs = new Dictionary<ItemType, GameObject>();
      foreach (var meeple in _itemPrefabEntries)
      {
        _itemPrefabs.Add(meeple.Type, meeple.Prefab);
      }

      // Create Inventory
      foreach (var item in _data.Items)
      {
        for (int i = 0; i < item.Amount; i++)
        {
          var newObject = GameObject.Instantiate(_itemPrefabs[item.Type], _itemZone.transform);
          newObject.transform.SetAsLastSibling();
        }
      }
    }

    [Serializable]
    public class ItemPrefabEntry
    {
      [field: SerializeField] public ItemType Type { get; private set; }
      [field: SerializeField] public GameObject Prefab { get; private set; }
    }
  }
}
