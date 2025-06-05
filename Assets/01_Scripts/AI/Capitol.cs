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
    [YarnCommand("capitol_initial_shipment")]
    public void InitialCapitolShipment()
    {
      foreach (var colony in _colonies)
      {
        colony.InitialCapitolShipment();
      }
    }
    [YarnCommand("capitol_weekly_shipment")]
    public void WeeklyCapitolShipment()
    {
      foreach (var colony in _colonies)
      {
        colony.WeeklyCapitolShipment();
      }
    }
  }
}
