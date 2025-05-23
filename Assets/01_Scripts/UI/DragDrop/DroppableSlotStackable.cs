using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class DroppableSlotStackable : DroppableSlot
  {
    private int _maxAmount;
    private Stack<DraggableUI> _innerStack;
    public void Initialize(int maxAmount)
    {
      _maxAmount = maxAmount;
      _innerStack = new Stack<DraggableUI>();
    }

    public override void OnDrop(PointerEventData eventData)
    {
      if (transform.childCount < _maxAmount)
      {
        DraggableUI draggable = eventData.pointerDrag.GetComponent<DraggableUI>();
        if (MetadataValidator == null || MetadataValidator.Validate(draggable.Metadata))
        {
          if (_innerStack.Count > 0)
          {
            _innerStack.Peek().SetCount(0);
          }
          draggable.OnDroppingInto(this, _innerStack.Count + 1);
        }
      }
    }

    public override void SetIn(DraggableUI draggable)
    {
      draggable.transform.SetParent(transform);
      draggable.transform.localPosition = Vector3.zero;
      draggable.transform.SetAsLastSibling();
      _innerStack.Push(draggable);
      foreach (var subscriber in _onAddSubscriptions)
      {
        subscriber.Invoke(this, draggable);
      }
    }

    public override void UnsetIn(DraggableUI draggable)
    {
      _innerStack.Pop();
      if (_innerStack.Count > 0)
      {
        _innerStack.Peek().SetCount(_innerStack.Count);
      }
      foreach (var subscriber in _onRemoveSubscriptions)
      {
        subscriber.Invoke(this, draggable);
      }
    }

    public override void ClearChildren()
    {
      _innerStack.Clear();
      base.ClearChildren();
    }
  }
}
