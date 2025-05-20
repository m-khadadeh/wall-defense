using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    [SerializeField] private UnityEngine.UI.Image _image;
    private DroppableUI _currentDroppable;
    private DroppableUI _nextDroppable;
    [field: SerializeField] public Metadata Metadata { get; set; }
    public void OnBeginDrag(PointerEventData eventData)
    {
      //Debug.Log("Begin Drag");
      _currentDroppable.UnsetIn(this);
      _nextDroppable = null;
      transform.SetParent(transform.root);
      transform.SetAsLastSibling();
      _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
      //Debug.Log("Dragging");
      transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      //Debug.Log("End Drag");
      _nextDroppable ??= _currentDroppable;
      _nextDroppable.SetIn(this);
      _currentDroppable = _nextDroppable;
      _image.raycastTarget = true;
    }

    public void OnDroppingInto(DroppableUI droppable)
    {
      _nextDroppable = droppable;
    }

    private void Awake()
    {
      _currentDroppable = transform.parent.GetComponent<DroppableUI>();
      if (_currentDroppable == null)
      {
        Debug.LogError($"{gameObject} initalized draggable without home droppable.");
      }
    }
  }
}