using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "Task", menuName = "Scriptable Objects/Tasks/Task")]
  public class Task : ScriptableObject
  {
    public enum YieldType
    {
      Resource, Information, WallDefenseStateChange, WallHPChange
    }
    [field: SerializeField] public string TaskName { get; private set; }
    [field: SerializeField] public string TaskDescription { get; private set; }
    [SerializeField] private List<ItemRequirement> _requirements;
    [SerializeField] private List<YieldData> _yieldPossibilities;
    [SerializeField] private List<TimeToCompleteData> _timeToCompletePossibilities;
    [SerializeField] private List<TMPro.TMP_Dropdown.OptionData> _additionalChoices;

    private List<ItemType> _consumedItems;

    [System.Serializable]
    public class YieldData
    {
      [field: SerializeField] public TaskYield[] Yield { get; private set; }
      [field: SerializeField] public InventoryData.ItemAmountEntry[] ItemAmounts { get; private set; }
      [field: SerializeField] public string AdditionalChoice { get; private set; }
    }

    [System.Serializable]
    public class TimeToCompleteData
    {
      [field: SerializeField] public int Hours { get; private set; }
      [field: SerializeField] public InventoryData.ItemAmountEntry[] ItemAmounts { get; private set; }
      [field: SerializeField] public string AdditionalChoice { get; private set; }
    }

    public void Initialize()
    {
      foreach (var requirement in _requirements)
      {
        requirement.Reset();
      }
    }

    public void CompleteTask(ColonyData data, string choice)
    {
      _consumedItems = new List<ItemType>();
      foreach (var requirement in _requirements)
      {
        foreach (var item in requirement.CurrentItems)
        {
          if (requirement.IsConsumed)
          {
            _consumedItems.Add(item);
          }
        }
      }
      GetYield(data, choice, _consumedItems);
      ConsumeRequirements(data);
    }

    private void ConsumeRequirements(ColonyData data)
    {
      _consumedItems = new List<ItemType>();
      foreach (var requirement in _requirements)
      {
        foreach (var item in requirement.CurrentItems)
        {
          if (!requirement.IsConsumed)
          {
            data.Inventory.AddItem(item, 1);
          }
        }
        requirement.Reset();
      }
    }

    private void GetYield(ColonyData data, string choice, List<ItemType> consumed)
    {
      if (_yieldPossibilities.Count == 1)
      {
        // Skip the possibility list
        foreach (var yield in _yieldPossibilities[0].Yield)
        {
          yield.GetYield(data, choice, consumed);
        }
        return;
      }
      foreach (var entry in _yieldPossibilities)
      {
        if (CheckItemsAgainstPossibilityList(entry.ItemAmounts) && entry.AdditionalChoice == "")
        {
          foreach (var yield in entry.Yield)
          {
            yield.GetYield(data, choice, consumed);
          }
          return;
        }
      }
    }

    public int GetHours(string choice)
    {
      if (_timeToCompletePossibilities.Count == 1)
      {
        // Skip the possibility list
        return _timeToCompletePossibilities[0].Hours;
      }
      foreach (var entry in _timeToCompletePossibilities)
      {
        if (CheckItemsAgainstPossibilityList(entry.ItemAmounts) && entry.AdditionalChoice == "")
        {
          return entry.Hours;
        }
      }
      throw new Exception("Hours not within possibility lists.");
    }

    private bool CheckItemsAgainstPossibilityList(InventoryData.ItemAmountEntry[] possibilityList)
    {
      foreach (var requirement in _requirements)
      {
        var entry = Array.Find(possibilityList, possibility => possibility.Type == requirement.RequirementType);
        if (entry != null)
        {
          if (requirement.Count != entry.Amount)
          {
            return false;
          }  
        }
      }
      return true;
    }

    public void InitializeUI(List<TaskSlot> itemSlots, TMPro.TMP_Dropdown dropdown, Action checkAction)
    {
      if (_additionalChoices.Count > 0)
      {
        dropdown.gameObject.SetActive(true);
        dropdown.options = _additionalChoices;
      }
      else
      {
        dropdown.gameObject.SetActive(false);
      }
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
              foreach (var element in itemSlots)
              {
                if(element.gameObject.activeSelf)
                  element.CheckAutoFillAvailable();
              }
              checkAction.Invoke();
            },
            (DroppableUI droppable, DraggableUI draggable) =>
            {
              requirement.Remove(draggable.Metadata.Type);
              foreach (var element in itemSlots)
              {
                if(element.gameObject.activeSelf)
                  element.CheckAutoFillAvailable();
              }
              checkAction.Invoke();
            }
          );
        requirement.InitializeUI(() =>
            {
              slot.Droppable.ClearChildren();
              foreach (var element in itemSlots)
              {
                if(element.gameObject.activeSelf)
                  element.CheckAutoFillAvailable();
              }
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
