using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using Yarn.Unity;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "SettlementAI", menuName = "Scriptable Objects/AI/SettlementAI")]
  public class SettlementAI : ScriptableObject
  {
    private VariableStorageBehaviour _variableStorage => _dialogueManager.VariableStorage;
    private ColonyData _colony;
    [SerializeField] private DialogueManager _dialogueManager;
    [field: SerializeField] public string VariableNamePrefix { get; private set; }
    [SerializeField] private List<string> _acceptableStressLevelsForRequest;
    [SerializeField] private List<string> _acceptableFriendLevelsForRequest;
    public Dictionary<string, Planner.ResourceTally> SparableAmounts { get; set; }
    [SerializeField] private AnimationCurve _wallHPStressCurve;
    [SerializeField] private int _maxStressPerSegment;
    private Dictionary<ItemType, ItemPromise> _promises;
    [SerializeField] private int _friendShipPerItemGift;
    [SerializeField] private int _friendShipPerPromiseKept;
    [SerializeField] private int _friendShipPerPromiseUnkept;
    [SerializeField] private string _promiseKeptNode;
    [SerializeField] private string _promiseUnkeptNode;
    [SerializeField] private string _askForItemsNode;
    [SerializeField] private string _askForGhoulInfoNode;
    [SerializeField] private int _sidesAskForItems;
    [SerializeField] private int _sidesAskForGhoulInfo;
    [SerializeField] private WorldState _ghoulInfo;

    private bool _askedForItemsToday;
    private bool _askedForInfoToday;
    private int _currentHour;
    public void Initialize(ColonyData data)
    {
      _colony = data;
      _colony.Subscribe(OnGiftReceived);
      _variableStorage.SetValue($"${VariableNamePrefix}_station", _colony.StationName);
      _promises = new Dictionary<ItemType, ItemPromise>();
    }

    public void OnDayStart()
    {
      _askedForInfoToday = false;
      _askedForItemsToday = false;
    }

    public void OnBeforeHour(int hour)
    {
      if (!_askedForItemsToday)
      {
        if (Random.Range(0, _sidesAskForItems) == 0)
        {
          SetRequiredMaterialsVariable();
          _dialogueManager.QueueUpDialogue(_askForItemsNode, hour + 1);
          _askedForItemsToday = true;
        }
      }
      if (!_askedForInfoToday && hour > 10 && _ghoulInfo.StateValue > 1)
      {
        if (Random.Range(0, _sidesAskForGhoulInfo) == 0)
        {
          SetRequiredMaterialsVariable();
          _dialogueManager.QueueUpDialogue(_askForGhoulInfoNode, hour + 1);
          _askedForInfoToday = true;
        }
      }
    }

    public void OnAfterHour(int hour)
    {
      _currentHour = hour;
      UpdateStress();
      int friendShipAmount = 0;
      List<ItemType> _promisesUnkept = new();
      foreach (var promise in _promises)
      {
        if (promise.Value.TickDown())
        {
          friendShipAmount += _friendShipPerPromiseUnkept;
          _promisesUnkept.Add(promise.Key);
        }
      }
      foreach (var promise in _promisesUnkept)
      {
        _promises.Remove(promise);
      }
      if (_promisesUnkept.Count > 0)
      {
        _dialogueManager.QueueUpDialogue(_promiseUnkeptNode, _currentHour + 1);
      }
    }
    private void OnGiftReceived(List<ItemType> items)
    {
      Dictionary<ItemType, int> counts = new Dictionary<ItemType, int>();
      List<ItemType> _promisesKept = new();
      int friendShipAmount = 0;
      foreach (var item in items)
      {
        if (counts.ContainsKey(item))
        {
          counts[item]++;
        }
        else
        {
          counts[item] = 1;
        }
        friendShipAmount += _friendShipPerItemGift;
      }
      foreach (var promise in _promises)
      {
        if (counts.TryGetValue(promise.Key, out int amount))
        {
          if (amount >= promise.Value.Amount)
          {
            _promisesKept.Add(promise.Key);
            friendShipAmount += _friendShipPerPromiseKept;
          }
        }
      }
      foreach (var item in _promisesKept)
      {
        _promises.Remove(item);
      }
      if (_promisesKept.Count > 0)
      {
        _dialogueManager.QueueUpDialogue(_promiseKeptNode, _currentHour + 1);
      }
      _variableStorage.TryGetValue<int>($"{VariableNamePrefix}_relationship", out int currentFriendship);
      _variableStorage.SetValue($"{VariableNamePrefix}_relationship", currentFriendship + friendShipAmount);
    }
    public void OnClueReceivedViaDialogue(string clueName) => _colony.OnClueReceivedViaDialogue(clueName);
    public bool RequestMaterial(string type, int amount)
    {
      bool allowedStress = false;
      foreach (var level in _acceptableStressLevelsForRequest)
      {
        _variableStorage.TryGetValue<bool>($"{VariableNamePrefix}_stress_{level}", out bool levelValue);
        allowedStress |= levelValue;
      }
      bool allowedFriend = false;
      foreach (var level in _acceptableStressLevelsForRequest)
      {
        _variableStorage.TryGetValue<bool>($"{VariableNamePrefix}_relationship_{level}", out bool levelValue);
        allowedFriend |= levelValue;
      }
      return allowedStress && allowedFriend && SparableAmounts[type].SpareableAmount <= amount;
    }
    public void UpdateStress()
    {
      int stress = 0;
      stress += (int)(_wallHPStressCurve.Evaluate((float)_colony.Wall.bottom.health / (float)_colony.Wall.bottom.maxhealth) * _maxStressPerSegment);
      stress += (int)(_wallHPStressCurve.Evaluate((float)_colony.Wall.middle.health / (float)_colony.Wall.middle.maxhealth) * _maxStressPerSegment);
      stress += (int)(_wallHPStressCurve.Evaluate((float)_colony.Wall.top.health / (float)_colony.Wall.top.maxhealth) * _maxStressPerSegment);
      _variableStorage.SetValue($"{VariableNamePrefix}_stress", stress);
    }
    public void SetRequiredMaterialsVariable()
    {
      int amount = int.MaxValue;
      string resourceType = "";
      foreach (var material in SparableAmounts)
      {
        if (material.Value.SpareableAmount < amount)
        {
          amount = material.Value.SpareableAmount;
          resourceType = material.Value.DialogueKey;
        }
      }
      _variableStorage.SetValue($"requested_asset", resourceType);
      _variableStorage.SetValue($"requested_amount", Random.Range(4, 6));
    }

    public void OnPromise(ItemType type, int amount, int time)
    {
      _promises[type] = new ItemPromise(type, amount, time);
    }

    private class ItemPromise
    {
      private ItemType _item;
      private int _amount;
      private int _time;

      public ItemType Item => _item;
      public int Amount => _amount;

      public ItemPromise(ItemType item, int amt, int time)
      {
        _item = item;
        _amount = amt;
        _time = time;
      }

      public bool TickDown()
      {
        _time--;
        return _time <= 0;
      }
    }
  }
}

