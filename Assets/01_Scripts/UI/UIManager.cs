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
      _taskManager.Initialize(null);
    }

    public void TestDialogueBox()
    {
      DialogBox.QueueDialogueBox(new DialogueBoxParameters
      {
        parent = transform,
        prompt = "Hey, test out this dialogue box!",
        choices = new string[] { "Sure!", "No :[" },
        eventHandlers = new DialogBox.ButtonEventHandler[] {
          () =>
          {
            Debug.Log("Thanks!");
          },
          () =>
          {
            Debug.Log("Fuck you!");
          }
        }
      }
      );
    }
  }
}
