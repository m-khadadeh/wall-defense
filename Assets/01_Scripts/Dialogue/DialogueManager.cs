using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "DialogueManager", menuName = "Scriptable Objects/DialogueManager")]
  public class DialogueManager : ScriptableObject
  {
    private DialogueRunner _runner;
    private Dictionary<int, List<string>> _queuedDialogueNodes;
    public VariableStorageBehaviour VariableStorage => _runner.VariableStorage;
    private Dictionary<string, ColonyData> _aiColonies;
    private ColonyData _playerColony;
    [SerializeField] private AI.SettlementAI _fairweatherAI;
    private int _currentHour;
    [SerializeField] private List<ResourceEntry> _resourceKeys;
    private Dictionary<string, ItemType> _resourceDictionary;
    [SerializeField] private string _playerInitiatesNode;
    private List<Deliverable> _deliveries;
    public bool NodeQueued { get; private set; }
    private string _nextNode;
    private GameManager.UIView _currentView;
    public void Initialize(DialogueRunner runner, List<ColonyData> aiColonies, ColonyData playerColony, System.Action continueSetup, int currentHour)
    {
      _currentHour = currentHour;
      NodeQueued = false;
      _runner = runner;
      _nextNode = "";
      _queuedDialogueNodes = new Dictionary<int, List<string>>();
      _playerColony = playerColony;
      _aiColonies = aiColonies.ToDictionary(e => e.AIController.name, e => e);
      _resourceDictionary = _resourceKeys.ToDictionary(e => e.Key, e => e.Item);
      ResetDictionary();
      _deliveries = new List<Deliverable>();
      _runner.onDialogueComplete.AddListener(() =>
      {
        SetStartDialogueVariables();
        continueSetup.Invoke();
      });
      _runner.StartDialogue("Variable_Declaration");
    }

    public void Unbind()
    {
      _runner = null;
    }

    private void ResetDictionary()
    {
      for (int i = 0; i < 24; i++)
      {
        _queuedDialogueNodes[i] = new List<string>();
      }
    }

    public void OnAfterHour(int hour, GameManager.UIView view)
    {
      _currentHour = hour;
      _currentView = view;
      List<Deliverable> toDelete = new List<Deliverable>();
      foreach (var deliverable in _deliveries)
      {
        if (deliverable.TickDownAndDeliver())
        {
          toDelete.Add(deliverable);
        }
      }
      foreach (var deliverable in toDelete)
      {
        _deliveries.Remove(deliverable);
      }
      QueueUpNextNode();
      if (view == GameManager.UIView.morse)
      {
        RunNextNode();
      }
    }

    private void QueueUpNextNode()
    {
      if (_queuedDialogueNodes[_currentHour].Count > 0)
      {
        NodeQueued = true;
        _nextNode = _queuedDialogueNodes[_currentHour][0];
        _queuedDialogueNodes[_currentHour].RemoveAt(0);
      }
      else
      {
        NodeQueued = false;
        _nextNode = "";
      }
    }

    public async void RunNextNode()
    {
      await YarnTask.Delay(20);
      if (_nextNode == "")
      {
        QueueUpNextNode();
      }
      if (_nextNode != "")
      {
        _runner.StartDialogue(_nextNode);
        _nextNode = "";
      }
    }

    public void RunPlayerInititation()
    {
      _runner.StartDialogue(_playerInitiatesNode);
    }

    public void OnScreenEntry()
    {
      if (_nextNode == "")
      {
        RunPlayerInititation();
      }
      else
      {
        RunNextNode();
      }
    }

    public void QueueUpDialogue(string nodeName, int hour)
    {
      if (!_queuedDialogueNodes[hour].Contains(nodeName))
        _queuedDialogueNodes[hour].Add(nodeName);
    }

    private void SetStartDialogueVariables()
    {
      VariableStorage.SetValue("$player_station_name", _playerColony.StationName);
      VariableStorage.SetValue("$player_name", _playerColony.PlayerName);
      VariableStorage.SetValue("$player_wall_name", _playerColony.Wall.wallName);
      VariableStorage.SetValue("$fairweather_ai_object_name", _fairweatherAI.name);
      _runner.onDialogueComplete.RemoveAllListeners();
      _runner.onDialogueComplete.AddListener(RunNextNode);
    }

    public void RequestMaterial(string aiObjectName, string materialType, int amount)
    {
      if (_aiColonies[aiObjectName].AIController.RequestMaterial(materialType, amount))
      {
        _aiColonies[aiObjectName].Inventory.RemoveItem(_resourceDictionary[materialType], amount);
        _deliveries.Add(new Deliverable(_resourceDictionary[materialType], amount, 3, _playerColony));
        VariableStorage.SetValue($"${_aiColonies[aiObjectName].AIController.VariableNamePrefix}_ask_for_resources_response", true);
      }
      VariableStorage.SetValue($"${_aiColonies[aiObjectName].AIController.VariableNamePrefix}_ask_for_resources_response", false);
    }

    public void GetRequiredMaterials(string aiObjectName)
    {
      _aiColonies[aiObjectName].AIController.SetRequiredMaterialsVariable();
    }

    public void MakePromise(string aiObjectName, string materialType, int amount)
    {
      _aiColonies[aiObjectName].AIController.OnPromise(_resourceDictionary[materialType], amount, 6);
    }

    public void GiveInfo(string aiObjectName, string clue)
    {
      _aiColonies[aiObjectName].AIController.OnClueReceivedViaDialogue(clue);
    }

    [Serializable]
    public class ResourceEntry
    {
      [field: SerializeField] public string Key { get; private set; }
      [field: SerializeField] public ItemType Item { get; private set; }
    }

    private class Deliverable
    {
      private ItemType _item;
      private int _amount;
      private int _time;
      private ColonyData _deliverTo;

      public Deliverable(ItemType type, int amt, int time, ColonyData destination)
      {
        _item = type;
        _amount = amt;
        _time = time;
        _deliverTo = destination;
      }

      public bool TickDownAndDeliver()
      {
        if (_time == 0)
        {
          _deliverTo.Inventory.AddItem(_item, _amount);
          return true;
        }
        else
        {
          _time--;
          return false;
        }
      }
    }

    public void QueueNodeNow(string node)
    {
      _nextNode = node;
    }
  }
}
