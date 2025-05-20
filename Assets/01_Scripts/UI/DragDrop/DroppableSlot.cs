using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class DroppableSlot : MonoBehaviour, DroppableUI
  {
    public void SetIn(DraggableUI draggable)
    {
      draggable.transform.SetParent(transform);
      draggable.transform.localPosition = Vector3.zero;
    }

    public void OnDrop(PointerEventData eventData)
    {
      if(transform.childCount == 0)
      {
        DraggableUI draggable = eventData.pointerDrag.GetComponent<DraggableUI>();
        draggable.OnDroppingInto(this);
      }
    }
  }
}