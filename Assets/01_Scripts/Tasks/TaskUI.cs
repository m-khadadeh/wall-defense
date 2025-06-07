using System;
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
    [SerializeField] private TMPro.TextMeshProUGUI _runButtonText;
    [SerializeField] private ColonyData _colonyData;
    [SerializeField] private TMPro.TMP_Dropdown _additionalChoice;
    private int _hoursRemaining;
    private string _taskChoice;
    public bool TaskRunning => _hoursRemaining > 0;
    public int HoursRemaining => _hoursRemaining;
    private Action _upDateWatch;

    public void Initialize(Action updateWatch)
    {
      _task.Initialize();
      _textMesh.text = _task.TaskName;
      _task.InitializeUI(_itemSlots, _additionalChoice, CheckFulfilled);
      _upDateWatch = updateWatch;
      CheckFulfilled();
      _hoursRemaining = -1;
      SetButtons();
    }

    public void CheckFulfilled()
    {
      _runButton.interactable = _task.IsFulfilled;
    }

    public void CheckAutoFillButtons()
    {
      foreach (var slot in _itemSlots)
      {
        if (slot.gameObject.activeSelf)
          slot.CheckAutoFillAvailable();
      }
    }

    public void OnIncrementHour()
    {
      if (TaskRunning)
      {
        _hoursRemaining--;
        CheckZeroHours();
      }
    }

    private void CheckZeroHours()
    {
      if (_hoursRemaining == 0)
      {
        // Task ends
        _task.CompleteTask(_colonyData, _taskChoice);
        CheckFulfilled();
        _hoursRemaining = -1;
        LockResources(false);
      }
      SetButtons();
    }

    private void SetButtons()
    {
      _additionalChoice.interactable = !TaskRunning;
      if (TaskRunning)
      {
        _runButtonText.text = $"{_hoursRemaining} Hours";
      }
      else
      {
        _runButtonText.text = "Approve";
      }
    }

    public void RunTask()
    {
      // TODO: Implement with Day/Night cycle system
      _taskChoice = _additionalChoice.options[_additionalChoice.value].text;
      _hoursRemaining = _task.GetHours(_taskChoice);
      _runButton.interactable = false;
      LockResources(true);
      SetButtons();
      CheckZeroHours();
      _upDateWatch?.Invoke();
    }

    private void LockResources(bool isLocked)
    {
      foreach (var slot in _itemSlots)
      {
        slot.LockSlot(isLocked);
      }
    }
  }
}
