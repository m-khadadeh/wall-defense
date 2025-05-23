using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public class TaskUI : MonoBehaviour
  {
    [SerializeField] private Task _task;
    [SerializeField] private List<TaskSlot> _itemSlots;
    [SerializeField] private TMPro.TextMeshProUGUI _textMesh;
    public void Start()
    {
      _task.Initialize();
      Initialize();
    }

    public void Initialize()
    {
      _textMesh.text = _task.name;
      _task.InitializeUI(_itemSlots, CheckFulfilled);
    }

    public void CheckFulfilled()
    {
      if (_task.IsFulfilled)
        Debug.Log("Fulfilled");
      else
        Debug.Log("Unfulfilled");
    }
  }
}
