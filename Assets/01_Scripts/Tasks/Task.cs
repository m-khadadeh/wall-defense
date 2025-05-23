using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "Task", menuName = "Scriptable Objects/Task")]
  public class Task : ScriptableObject
  {
    public enum YieldType
    {
      Resource, Information, WallDefenseStateChange, WallHPChange
    }
    [SerializeField] private List<ItemRequirement> _requirements;
    [SerializeField] private YieldType _taskYieldType;
    [SerializeField] private List<YieldData> _resourceYieldPossibilities;
    [SerializeField] private List<TimeToCompleteData> _timeToCompletePossibilities;

    [System.Serializable]
    public class YieldData
    {
      [field: SerializeField] public ItemType[] Type { get; private set; }
      [field: SerializeField] public Vector2Int[] MinMaxlYield { get; private set; }
      [field: SerializeField] public bool[] OptionalMeepleRequirment { get; private set; }
      [field: SerializeField] public bool[] OptionalMaterialRequirment { get; private set; }
    }

    [System.Serializable]
    public class TimeToCompleteData
    {
      [field: SerializeField] public int Hours { get; private set; }
      [field: SerializeField] public bool[] OptionalMeepleRequirment { get; private set; }
    }

    public void Initialize()
    {
      foreach (var requirement in _requirements)
      {
        requirement.Initialize();
      }
    }

    public void InitializeUI(List<TaskSlot> itemSlots, Action checkAction)
    {
      foreach (var slot in itemSlots)
      {
        slot.gameObject.SetActive(false);
      }
      for (int i = 0; i < _requirements.Count; i++)
      {
        var requirement = _requirements[i];
        itemSlots[i].gameObject.SetActive(true);
        itemSlots[i].InitializeSlot(requirement);
        itemSlots[i].Droppable.Subscribe(
            (DroppableUI droppable, DraggableUI draggable) =>
            {
              requirement.Add();
              checkAction.Invoke();
            },
            (DroppableUI droppable, DraggableUI draggable) =>
            {
              requirement.Remove();
              checkAction.Invoke();
            });
      }
    }

    public bool IsFulfilled
    {
      get
      {
        foreach (var requirement in _requirements)
        {
          if (!requirement.IsFulfilled)
            return false;
        }
        return true;
      }
    }
  }
}
