using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public interface DroppableUI : IDropHandler
  {
    public void SetIn(DraggableUI draggable);
  }
}