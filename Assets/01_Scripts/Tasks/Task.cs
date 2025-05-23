using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private List<YieldData> _yieldPossibilities;
    [SerializeField] private List<TimeToCompleteData> _timeToCompletePossibilities;

    [System.Serializable]
    public class YieldData
    {
      [field: SerializeField] public TaskYield[] Yield { get; private set; }
      [field: SerializeField] public InventoryData.ItemAmountEntry[] ItemAmounts { get; private set; }
    }

    [System.Serializable]
    public class TimeToCompleteData
    {
      [field: SerializeField] public int Hours { get; private set; }
      [field: SerializeField] public InventoryData.ItemAmountEntry[] ItemAmounts { get; private set; }
    }

    public void Initialize()
    {
      foreach (var requirement in _requirements)
      {
        requirement.Reset();
      }
    }

    public void CompleteTask(ColonyData data)
    {
      GetYield(data);
      ConsumeRequirements(data);
    }

    private void ConsumeRequirements(ColonyData data)
    {
      foreach (var requirement in _requirements)
      {
        if (!requirement.IsConsumed)
        {
          foreach (var item in requirement.CurrentItems)
          {
            data.Inventory.AddItem(item, 1);
          }
        }
        requirement.Reset();
      }
    }

    private void GetYield(ColonyData data)
    {
      if (_yieldPossibilities.Count == 1)
      {
        // Skip the possibility list
        foreach (var yield in _yieldPossibilities[0].Yield)
        {
          yield.GetYield(data);
        }
        return;
      }
      foreach (var entry in _yieldPossibilities)
        {
          if (CheckItemsAgainstPossibilityList(entry.ItemAmounts))
          {
            foreach (var yield in entry.Yield)
            {
              yield.GetYield(data);
            }
            return;
          }
        }
    }

    private bool CheckItemsAgainstPossibilityList(InventoryData.ItemAmountEntry[] possibilityList)
    {
      foreach (var requirement in _requirements)
      {
        var entry = Array.Find(possibilityList, possibility => possibility.Type == requirement.RequirementType);
        int amountNeeded = entry != null ? entry.Amount : 0;
        if (requirement.Count != amountNeeded)
        {
          return false;
        }
      }
      return true;
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
        var slot = itemSlots[i];
        slot.gameObject.SetActive(true);
        slot.InitializeSlot(requirement);
        slot.Droppable.Subscribe(
            (DroppableUI droppable, DraggableUI draggable) =>
            {
              requirement.Add(draggable.Metadata.Type);
              checkAction.Invoke();
            },
            (DroppableUI droppable, DraggableUI draggable) =>
            {
              requirement.Remove(draggable.Metadata.Type);
              checkAction.Invoke();
            }
          );
        requirement.InitializeUI(() =>
            {
              slot.Droppable.ClearChildren();
            }
          );
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
