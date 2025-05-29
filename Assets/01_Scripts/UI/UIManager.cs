using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public class UIManager : MonoBehaviour
  {
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private TaskManager _taskManager;

    private void Awake()
    {
      List<Action> addCallbacks = new List<Action>() { _taskManager.CheckAllTaskAutoButtons };
      List<Action> removeCallbacks = new List<Action>() { _taskManager.CheckAllTaskAutoButtons };
      _inventoryUI.Initialize(addCallbacks, removeCallbacks);
      _taskManager.Initialize();
    }
  }
}
