using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    private AI.SettlementAI _fairweatherAI;
    private int _currentHour;
    [SerializeField] private List<ResourceEntry> _resourceKeys;
    private Dictionary<string, ItemType> _resourceDictionary;
    private List<Deliverable> _deliveries;
    public void Initialize(DialogueRunner runner, List<ColonyData> aiColonies, ColonyData playerColony)
    {
      _runner = runner;
      _queuedDialogueNodes = new Dictionary<int, List<string>>();
      _playerColony = playerColony;
      _aiColonies = aiColonies.ToDictionary(e => e.AIController.name, e => e);
      _runner.StartDialogue("Variable_Declaration");
      _runner.onDialogueComplete.AddListener(RunNextNode);
      _deliveries = new List<Deliverable>();
      _resourceDictionary = _resourceDictionary.ToDictionary(e => e.Key, e => e.Value);
      SetStartDialogueVariables();
      ResetDictionary();
    }

    private void ResetDictionary()
    {
      for (int i = 0; i < 24; i++)
      {
        _queuedDialogueNodes[i] = new List<string>();
      }
    }

    public void OnAfterHour(int hour)
    {
      _currentHour = hour;
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
      RunNextNode();
    }

    private void RunNextNode()
    {
      if (_queuedDialogueNodes[_currentHour].Count > 1)
      {
        string node = _queuedDialogueNodes[_currentHour][0];
        _queuedDialogueNodes[_currentHour].RemoveAt(0);
        _runner.StartDialogue(node);
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
    }

    [YarnCommand("request_material")]
    public void RequestMaterial(string aiObjectName, string materialType, int amount)
    {
      if (_aiColonies[aiObjectName].AIController.RequestMaterial(materialType, amount))
      {
        _aiColonies[aiObjectName].Inventory.RemoveItem(_resourceDictionary[materialType], amount);
        _deliveries.Add(new Deliverable(_resourceDictionary[materialType], amount, 3, _playerColony));
        VariableStorage.SetValue($"{_aiColonies[aiObjectName].AIController.VariableNamePrefix}_ask_for_resources_response", true);
      }
      VariableStorage.SetValue($"{_aiColonies[aiObjectName].AIController.VariableNamePrefix}_ask_for_resources_response", false);
    }

    [YarnCommand("get_required_materials")]
    public void GetRequiredMaterials(string aiObjectName)
    {
      _aiColonies[aiObjectName].AIController.SetRequiredMaterialsVariable();
    }

    [YarnCommand("make_promise")]
    public void MakePromise(string aiObjectName, string materialType, int amount)
    {
      _aiColonies[aiObjectName].AIController.OnPromise(_resourceDictionary[materialType], amount, 6);
    }

    [YarnCommand("give_information")]
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
  }
}
