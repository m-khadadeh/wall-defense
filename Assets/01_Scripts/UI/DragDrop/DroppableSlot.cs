using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class DroppableSlot : DroppableUI
  {
    public override void OnDrop(PointerEventData eventData)
    {
      if (transform.childCount == 0)
      {
        DraggableUI draggable = eventData.pointerDrag.GetComponent<DraggableUI>();
        if (MetadataValidator == null || MetadataValidator.Validate(draggable.Metadata))
        {
          draggable.OnDroppingInto(this);
        }
      }
    }

    public override void SetIn(DraggableUI draggable)
    {
      draggable.transform.SetParent(transform);
      foreach (var subscriber in _onAddSubscriptions)
      {
        subscriber.Invoke(this, draggable);
      }
    }

    public override void UnsetIn(DraggableUI draggable)
    {
      foreach (var subscriber in _onRemoveSubscriptions)
      {
        subscriber.Invoke(this, draggable);
      }
    }

    private void Awake()
    {
      _onAddSubscriptions = new List<Action<DroppableUI, DraggableUI>>();
      _onRemoveSubscriptions = new List<Action<DroppableUI, DraggableUI>>();
    }
  }
}