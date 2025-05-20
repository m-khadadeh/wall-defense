using UnityEngine;

namespace WallDefense
{
  public class SlotTester : MonoBehaviour
  {
    [SerializeField] bool _unsub = false;
    DroppableUI.Unsubscriber _unsubscriber;
    void Start()
    {
      var droppable = gameObject.GetComponent<DroppableUI>();
      string gameObjectName = gameObject.name;
      _unsubscriber = droppable.Subscribe(OnSet, OnUnset);
    }

    void OnSet(DroppableUI drop, DraggableUI drag)
    {
      Debug.Log($"{drag.gameObject.name} just got slotted into {drop.gameObject}.");
    }

    void OnUnset(DroppableUI drop, DraggableUI drag)
    {
      Debug.Log($"{drag.gameObject.name} just got unslotted out of {drop.gameObject}.");
    }

    void Update()
    {
      if (_unsub)
      {
        _unsub = false;
        if (_unsubscriber != null)
        {
          _unsubscriber.Unsubscribe();
          _unsubscriber = null;
        }
      }
    }
  }
}
