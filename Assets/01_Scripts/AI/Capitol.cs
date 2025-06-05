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
      _dialogueManager.QueueUpDialogue("Capitol_Introduction", 7);
    }

    public void OnNewDay()
    {
      _dayCounter++;
      if (_dayCounter != 0 && _dayCounter % _everyXDays == 0)
      {
        _dialogueManager.QueueUpDialogue("Capitol_WeeklyShipment", 7);
      }
    }
  }
}
