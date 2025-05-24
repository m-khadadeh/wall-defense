using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WallDefense
{
  public class TaskUI : MonoBehaviour
  {
    [SerializeField] private Task _task;
    [SerializeField] private List<TaskSlot> _itemSlots;
    [SerializeField] private TMPro.TextMeshProUGUI _textMesh;
    [SerializeField] private Button _runButton;
    [SerializeField] private ColonyData _colonyData;

    public void Initialize()
    {
      _task.Initialize();
      _textMesh.text = _task.name;
      _task.InitializeUI(_itemSlots, CheckFulfilled);
      CheckFulfilled();
    }

    public void CheckFulfilled()
    {
      _runButton.interactable = _task.IsFulfilled;
    }

    public void CheckAutoFillButtons()
    {
      foreach (var slot in _itemSlots)
      {
        if(slot.gameObject.activeSelf)
          slot.CheckAutoFillAvailable();
      }
    }

    public void RunTask()
    {
      // TODO: Implement with Day/Night cycle system
      Debug.Log($"{_task.GetHours()} Hours.");
      _task.CompleteTask(_colonyData);
      CheckFulfilled();
    }
  }
}
