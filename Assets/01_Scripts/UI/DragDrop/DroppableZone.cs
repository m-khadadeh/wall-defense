using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class DroppableZone : DroppableUI
  {
    public override void OnDrop(PointerEventData eventData)
    {
      DraggableUI draggable = eventData.pointerDrag.GetComponent<DraggableUI>();
      if (draggable != null && (MetadataValidator == null || MetadataValidator.Validate(draggable.Metadata)))
      {
        draggable.OnDroppingInto(this);
      }
    }

    public override void SetIn(DraggableUI draggable)
    {
      draggable.transform.SetParent(transform);
      foreach (var subscriber in _onAddSubscriptions)
      {
        subscriber.Invoke(this, draggable);
      }
      // TODO: Add Sorting Logic and Sort
    }

    public override void UnsetIn(DraggableUI draggable)
    {
      foreach (var subscriber in _onRemoveSubscriptions)
      {
        subscriber.Invoke(this, draggable);
      }
    }
  }
}
