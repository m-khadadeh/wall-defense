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

    private Dictionary<ItemType, List<GameObject>> _itemObjects;

    private Dictionary<ItemType, GameObject> _itemPrefabs;
    void Start()
    {
      // Set up dictionaries
      _itemPrefabs = new Dictionary<ItemType, GameObject>();
      foreach (var item in _itemPrefabEntries)
      {
        _itemPrefabs.Add(item.Type, item.Prefab);
      }

      _data.Initialize(AddItemCallback, RemoveItemCallback);
      _itemObjects = new Dictionary<ItemType, List<GameObject>>();
      _itemZone.Subscribe(
          (DroppableUI droppable, DraggableUI draggable) =>
          {
            ItemType type = draggable.Metadata.Type;
            _data.DropItemIn(type);
            if (!_itemObjects.ContainsKey(type))
            {
              _itemObjects[type] = new List<GameObject>();
            }
            _itemObjects[type].Add(draggable.gameObject);
          },
          (DroppableUI droppable, DraggableUI draggable) =>
          {
            ItemType type = draggable.Metadata.Type;
            _data.DragItemOut(type);
            _itemObjects[type].Remove(draggable.gameObject);
          }
        );

      // Create Inventory
      foreach (var item in _data.CurrentItems)
      {
        AddItemCallback(item.Key, item.Value);
      }
    }

    public void AddItemCallback(ItemType type, int amount)
    {
      if (!_itemObjects.ContainsKey(type))
      {
        _itemObjects[type] = new List<GameObject>();
      }
      for (int i = 0; i < amount; i++)
      {
        var newObject = GameObject.Instantiate(_itemPrefabs[type], _itemZone.transform);
        newObject.transform.SetAsLastSibling();
        _itemObjects[type].Add(newObject);
      }
    }

    public void RemoveItemCallback(ItemType type, int amount)
    {
      for (int i = 0; i < amount; i++)
      {
        GameObject currentItem = _itemObjects[type][0];
        Destroy(currentItem);
        _itemObjects[type].RemoveAt(0);
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
