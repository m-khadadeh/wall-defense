using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class DroppableZone : MonoBehaviour, DroppableUI
  {
    public void OnDrop(PointerEventData eventData)
    {
      DraggableUI draggable = eventData.pointerDrag.GetComponent<DraggableUI>();
      draggable.OnDroppingInto(this);
    }

    public void SetIn(DraggableUI draggable)
    {
      draggable.transform.SetParent(transform);
      // TODO: Add Sorting Logic and Sort
    }
  }
}
