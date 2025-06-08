using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using WallDefense.AI;
using Yarn.Unity;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ColonyData", menuName = "Scriptable Objects/ColonyData")]
  public class ColonyData : ScriptableObject
  {
    [field: SerializeField] public InventoryData Inventory { get; private set; }
    [field: SerializeField] public Wall Wall { get; private set; }
    [field: SerializeField] public AI.SettlementAI AIController { get; set; }
    [SerializeField] private GhoulDiscernmentChalkboard _ghoulBoard;
    [SerializeField] private AI.Planner _planner;
    [field: SerializeField] public string StationName { get; private set; }
    [field: SerializeField] public string PlayerName { get; private set; }
    private List<Action<List<ItemType>>> _onGiftReceived;
    [SerializeField] private List<InventoryData.ItemAmountEntry> _initialCapitolShipment;
    [SerializeField] private List<InventoryData.ItemAmountEntry> _weeklyCapitolShipment;
    [SerializeField] private int _capitolShipmentsXWeekly;
    [SerializeField] private List<AfterTimeYield> _afterTimeYields;
    private int _weeksToShipment;
    [SerializeField] private bool _dead;

    public void Initialize()
    {
      _onGiftReceived = new List<Action<List<ItemType>>>();
      Wall.InitializeWalls();
      if (AIController != null)
      {
        _ghoulBoard.Initialize(AIController.VariableNamePrefix);
        AIController.Initialize(this);
        _planner.Initialize(AIController);
        _onGiftReceived.Add(AIController.OnGiftReceived);
      }
      foreach (var timeYield in _afterTimeYields)
      {
        timeYield.Initialize();
      }
      _weeksToShipment = _capitolShipmentsXWeekly;
    }

    public void OnBeforeHour(int hour)
    {
      if (!_dead)
      {
        AIController?.OnBeforeHour(hour);
      }
    }

    public void OnAfterHour(int hour)
    {
      if (!_dead)
      {
        _planner?.OnAfterHour(hour);
        AIController?.OnAfterHour(hour);
        foreach (var afterYield in _afterTimeYields)
        {
          afterYield.TickDown();
        }
      }
    }

    public void ReceiveGift(List<ItemType> items)
    {
      foreach (var item in items)
      {
        Debug.Log($"{name} received {item}");
        Inventory.AddItem(item, 1);
      }
      foreach (var subscriber in _onGiftReceived)
      {
        subscriber.Invoke(items);
      }
    }
    public void Subscribe(Action<List<ItemType>> subscriber)
    {
      if (!_onGiftReceived.Contains(subscriber))
      {
        _onGiftReceived.Add(subscriber);
      }
    }

    public int GetAIModifiedActionCost(int baseCost)
    {
      return baseCost;
    }

    public void OnClueReceivedViaDialogue(string clueName) => _ghoulBoard.OnClueReceivedViaDialogue(clueName);

    public void InitialCapitolShipment()
    {
      foreach (var item in _initialCapitolShipment)
      {
        Inventory.AddItem(item.Type, item.Amount);
      }
    }

    public void WeeklyCapitolShipment()
    {
      _weeksToShipment--;
      if (_weeksToShipment == 0)
      {
        foreach (var item in _weeklyCapitolShipment)
        {
          Inventory.AddItem(item.Type, item.Amount);
        }
        _weeksToShipment = _capitolShipmentsXWeekly;
      }
    }

    public void Die(string deathMessage)
    {
      if (AIController == null)
      {
        // Player is dead.
        DialogBox.QueueDialogueBox(
          new DialogueBoxParameters(
            GameObject.Find("Canvas").transform,
            $"<align=\"center\">-------------------------------------\n<size=40>DEFEAT</size>\n-------------------------------------\n<align=\"left\">{deathMessage}",
            new string[] {
              "Main Menu",
              "Quit"
            },
            new DialogBox.ButtonEventHandler[]
            {
              () => SceneManager.LoadScene(0),
              () => Application.Quit()
            }
          )
        );
      }
      else
      {
        AIController.OnDie();
      }
      _dead = true;
    }
  }
}
