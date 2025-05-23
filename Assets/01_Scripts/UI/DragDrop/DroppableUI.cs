using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  using DroppableSubscriber = Action<DroppableUI, DraggableUI>;

  public abstract class DroppableUI : MonoBehaviour, IDropHandler
  {
    protected List<DroppableSubscriber> _onAddSubscriptions;
    protected List<DroppableSubscriber> _onRemoveSubscriptions;
    public GameObject GameObject { get; protected set; }
    [field: SerializeField] public MetadataValidator MetadataValidator { get; set; }
    public abstract void UnsetIn(DraggableUI draggable);
    public abstract void SetIn(DraggableUI draggable);
    public abstract void OnDrop(PointerEventData eventData);

    public virtual void ClearChildren()
    {
      while (transform.childCount > 0)
      {
        DestroyImmediate(transform.GetChild(0).gameObject);
      }
    }

    /// <summary>
    /// <paramref name="addSubscriber"/> is triggered when SetIn() is called, when a draggable is dropped in this droppable.
    /// <paramref name="removeSubscriber"/> is triggered when UnsetIn() is called, when a draggable is removed from this droppable.
    /// </summary>
    /// <param name="addSubscriber"></param>
    /// <param name="removeSubscriber"></param>
    /// <returns>
    /// An Unsubscriber. Call its Unsubscribe() function to unsubscribe from this class.
    /// </returns>
    public Unsubscriber Subscribe(DroppableSubscriber addSubscriber, DroppableSubscriber removeSubscriber)
    {
      if (!_onAddSubscriptions.Contains(addSubscriber))
      {
        _onAddSubscriptions.Add(addSubscriber);
      }
      if (!_onRemoveSubscriptions.Contains(removeSubscriber))
      {
        _onRemoveSubscriptions.Add(removeSubscriber);
      }
      return new DroppableUI.Unsubscriber(
        addSub: addSubscriber,
        removeSub: removeSubscriber,
        addSubs: _onAddSubscriptions,
        removeSubs: _onRemoveSubscriptions
      );
    }
    public class Unsubscriber
    {
      private DroppableSubscriber _addSubscriber;
      private DroppableSubscriber _removeSubscriber;

      private List<DroppableSubscriber> _addSubscriptions;
      private List<DroppableSubscriber> _removeSubscriptions;

      public Unsubscriber(DroppableSubscriber addSub, DroppableSubscriber removeSub,
          List<DroppableSubscriber> addSubs, List<DroppableSubscriber> removeSubs)
      {
        _addSubscriber = addSub;
        _removeSubscriber = removeSub;
        _addSubscriptions = addSubs;
        _removeSubscriptions = removeSubs;
      }

      public void Unsubscribe()
      {
        if (_addSubscriber != null && _addSubscriptions.Contains(_addSubscriber))
        {
          _addSubscriptions.Remove(_addSubscriber);
        }
        if (_removeSubscriber != null && _removeSubscriptions.Contains(_removeSubscriber))
        {
          _removeSubscriptions.Remove(_removeSubscriber);
        }
      }
    }
    
    private void Awake()
    {
      _onAddSubscriptions = new List<DroppableSubscriber>();
      _onRemoveSubscriptions = new List<DroppableSubscriber>();
    }
  }
}