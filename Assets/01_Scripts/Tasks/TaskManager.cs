using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WallDefense
{
  public class TaskManager : MonoBehaviour
  {
    [SerializeField] private List<TaskUI> _tasksUIs;
    [SerializeField] private TMPro.TextMeshProUGUI _taskInfoName;
    [SerializeField] private TMPro.TextMeshProUGUI _taskInfoDescription;

    public void Initialize(Action watchUpdate)
    {
      foreach (var task in _tasksUIs)
      {
        task.Initialize(watchUpdate);
      }
    }
    public void OnIncrementHour()
    {
      foreach (var task in _tasksUIs)
      {
        task.OnIncrementHour();
      }
    }

    public void SetInfo(Task task)
    {
      if (task != null)
      {
        _taskInfoName.text = task.TaskDescriptionName;
        _taskInfoDescription.text = task.TaskDescription;
      }
      else
      {
        _taskInfoName.text = "";
        _taskInfoDescription.text = "";
      }
    }

    public void CheckAllTaskAutoButtons()
    {
      foreach (var task in _tasksUIs)
      {
        task.CheckAutoFillButtons();
      }
    }

    public int GetShortestTimeToTaskCompletion()
    {
      int lowestTime = int.MaxValue;
      int lowestIndex = -1;
      for (int i = 0; i < _tasksUIs.Count; i++)
      {
        if (_tasksUIs[i].TaskRunning && _tasksUIs[i].HoursRemaining < lowestTime)
        {
          lowestTime = _tasksUIs[i].HoursRemaining;
          lowestIndex = i;
        }
      }
      if (lowestIndex == -1)
      {
        return 0;
      }
      return lowestTime;
    }
  }
}
