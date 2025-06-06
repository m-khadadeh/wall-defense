using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "The Capitol", menuName = "Scriptable Objects/AI/Capitol")]
  public class Capitol : ScriptableObject
  {
    [SerializeField] private List<ColonyData> _colonies;
    [SerializeField] private DialogueManager _dialogueManager;
    private int _dayCounter;
    [SerializeField] private int _everyXDays;
    [field: SerializeField] public string PlayerInitiationNode { get; private set; }
    public void InitialCapitolShipment()
    {
      foreach (var colony in _colonies)
      {
        colony.InitialCapitolShipment();
      }
    }
    public void WeeklyCapitolShipment()
    {
      foreach (var colony in _colonies)
      {
        colony.WeeklyCapitolShipment();
      }
    }

    public void Initialize()
    {
      _dayCounter = -1;
      InitialDay();
    }

    private void InitialDay()
    {
      _dialogueManager.QueueUpDialogue("Capitol_Introduction", 6);
    }

    public void OnNewDay()
    {
      _dayCounter++;
      _dialogueManager.VariableStorage.SetValue("$days_till_capitol_shipment", _everyXDays - (_dayCounter % _everyXDays));
      if (_dayCounter != 0 && _dayCounter % _everyXDays == 0)
      {
        _dialogueManager.QueueUpDialogue("Capitol_WeeklyShipment", 6);
      }
    }
  }
}
