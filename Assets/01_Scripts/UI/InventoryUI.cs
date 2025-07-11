using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public class InventoryUI : MonoBehaviour
  {
    [SerializeField] private InventoryData _data;
    [SerializeField] private DroppableZoneStackable _itemZone;
    private Dictionary<ItemType, List<GameObject>> _itemObjects;

    public void Initialize(List<Action> addCallbacks, List<Action> removeCallbacks)
    {
      _itemZone.Initialize();

      _data.Initialize(
          (ItemType type, int amount) =>
          {
            AddItemCallback(type, amount);
            foreach (var callback in addCallbacks)
            {
              callback.Invoke();
            }
          },
          (ItemType type, int amount) =>
          {
            RemoveItemCallback(type, amount);
            foreach (var callback in removeCallbacks)
            {
              callback.Invoke();
            }
          }
        );
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
            foreach (var callback in addCallbacks)
            {
              callback.Invoke();
            }
          },
          (DroppableUI droppable, DraggableUI draggable) =>
          {
            ItemType type = draggable.Metadata.Type;
            _data.DragItemOut(type);
            _itemObjects[type].Remove(draggable.gameObject);
            foreach (var callback in removeCallbacks)
            {
              callback.Invoke();
            }
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
        var newObject = GameObject.Instantiate(type.Prefab, _itemZone.transform);
        _itemZone.AddItemWithoutCallback(newObject.GetComponent<DraggableUI>());
        _itemObjects[type].Add(newObject);
      }
    }

    public void RemoveItemCallback(ItemType type, int amount)
    {
      for (int i = 0; i < amount; i++)
      {
        GameObject currentItem = _itemObjects[type][0];
        _itemZone.DeleteItem(currentItem.GetComponent<DraggableUI>());
        _itemObjects[type].RemoveAt(0);
      }
    }
  }
}
