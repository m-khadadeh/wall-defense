using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    [SerializeField] private UnityEngine.UI.Image _image;
    [SerializeField] private TMPro.TextMeshProUGUI _numberText;
    private DroppableUI _currentDroppable;
    private DroppableUI _nextDroppable;
    [field: SerializeField] public Metadata Metadata { get; set; }
    public void OnBeginDrag(PointerEventData eventData)
    {
      //Debug.Log("Begin Drag");
      SetCount(1);
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

    public void OnDroppingInto(DroppableUI droppable, int stackSize = 1)
    {
      _nextDroppable = droppable;
      SetCount(stackSize);      
    }

    private void Awake()
    {
      _currentDroppable = transform.parent.GetComponent<DroppableUI>();
      if(_currentDroppable == null)
        _currentDroppable = transform.parent.parent.GetComponent<DroppableUI>();
      if (_currentDroppable == null)
        {
          Debug.LogError($"{gameObject} initalized draggable without home droppable.");
        }
    }

    public void SetCount(int count)
    {
      _numberText.text = (count > 1) ? count.ToString() : "";
    }
  }
}