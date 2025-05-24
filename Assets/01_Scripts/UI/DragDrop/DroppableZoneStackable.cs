using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class DroppableZoneStackable : DroppableZone
  {
    private int _maxAmount;
    private Dictionary<ItemType, Queue<DraggableUI>> _innerQueues;
    private Dictionary<ItemType, Transform> _hiddenParentTypes;
    [SerializeField] private Transform _viewableGridParent;
    [SerializeField] private Transform _hiddenParent;

    public void Initialize(int maxAmount = int.MaxValue)
    {
      _maxAmount = maxAmount;
      _innerQueues = new Dictionary<ItemType, Queue<DraggableUI>>();
      _hiddenParentTypes = new Dictionary<ItemType, Transform>();
      _hiddenParent.gameObject.SetActive(false);
      _viewableGridParent.gameObject.SetActive(true);
    }

    public override void OnDrop(PointerEventData eventData)
    {
      if (transform.childCount < _maxAmount)
      {
        DraggableUI draggable = eventData.pointerDrag.GetComponent<DraggableUI>();
        if (MetadataValidator == null || MetadataValidator.Validate(draggable.Metadata))
        {
          if (!_innerQueues.ContainsKey(draggable.Metadata.Type))
          {
            AddNewType(draggable.Metadata.Type);
          }
          if (_innerQueues[draggable.Metadata.Type].Count < _maxAmount)
          {
            draggable.OnDroppingInto(this);
          }
        }
      }
    }

    public override void SetIn(DraggableUI draggable)
    {
      AddItemWithoutCallback(draggable);
      foreach (var subscriber in _onAddSubscriptions)
      {
        subscriber.Invoke(this, draggable);
      }
    }

    public override void UnsetIn(DraggableUI draggable)
    {
      _innerQueues[draggable.Metadata.Type].Dequeue();
      if (_innerQueues[draggable.Metadata.Type].Count > 0)
      {
        var nextDraggable = _innerQueues[draggable.Metadata.Type].Peek();
        nextDraggable.transform.SetParent(_viewableGridParent);
        nextDraggable.transform.SetSiblingIndex(draggable.transform.GetSiblingIndex());
        nextDraggable.SetCount(_innerQueues[draggable.Metadata.Type].Count);
      }
      foreach (var subscriber in _onRemoveSubscriptions)
      {
        subscriber.Invoke(this, draggable);
      }
    }

    public void AddItemWithoutCallback(DraggableUI draggable)
    {
      if (!_innerQueues.ContainsKey(draggable.Metadata.Type))
      {
        AddNewType(draggable.Metadata.Type);
      }
      if (_innerQueues[draggable.Metadata.Type].Count > 0)
      {
        draggable.transform.position = _hiddenParent.position;
        draggable.transform.SetParent(_hiddenParentTypes[draggable.Metadata.Type]);
        _innerQueues[draggable.Metadata.Type].Enqueue(draggable);
        _innerQueues[draggable.Metadata.Type].Peek().SetCount(_innerQueues[draggable.Metadata.Type].Count);
      }
      else
      {
        draggable.transform.SetParent(_viewableGridParent);
        draggable.transform.SetAsLastSibling();
        draggable.SetCount(1);
        _innerQueues[draggable.Metadata.Type].Enqueue(draggable);
      }
    }

    public void DeleteItem(DraggableUI draggableUI)
    {
      var toDelete = _innerQueues[draggableUI.Metadata.Type].Dequeue();
      if (_innerQueues[draggableUI.Metadata.Type].Count > 0)
      {
        var nextDraggable = _innerQueues[draggableUI.Metadata.Type].Peek();
        nextDraggable.transform.SetParent(_viewableGridParent);
        nextDraggable.transform.SetSiblingIndex(draggableUI.transform.GetSiblingIndex());
        nextDraggable.SetCount(_innerQueues[draggableUI.Metadata.Type].Count);
      }
      Destroy(toDelete.gameObject);
    }

    private void AddNewType(ItemType type)
    {
      _innerQueues.Add(type, new Queue<DraggableUI>());

      GameObject newObject = new GameObject(type.name, typeof(RectTransform));
      newObject.transform.SetParent(_hiddenParent);
      _hiddenParentTypes.Add(type, newObject.transform);
    }
  }
}
