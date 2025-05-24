using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public class UIManager : MonoBehaviour
  {
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private List<TaskUI> _taskUIs;

    private void Awake()
    {
      List<Action> addCallbacks = new List<Action>() { CheckAllTaskAutoButtons };
      List<Action> removeCallbacks = new List<Action>() { CheckAllTaskAutoButtons };
      _inventoryUI.Initialize(addCallbacks, removeCallbacks);
      foreach (var task in _taskUIs)
      {
        task.Initialize();
      }
    }

    private void CheckAllTaskAutoButtons()
    {
      foreach (var task in _taskUIs)
      {
        task.CheckAutoFillButtons();
      }
    }
  }
}
