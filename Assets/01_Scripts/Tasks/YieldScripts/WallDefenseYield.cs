using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "WallDefenseYield", menuName = "Scriptable Objects/Tasks/Yields/Wall Defense Yield")]
  public class WallDefenseYield : TaskYield
  {
    [SerializeField] private DamageParameters.Type _defenseType;
    [SerializeField] private List<Entry> _wallSections;
    private Dictionary<string, WallSegmentName> _dictionary;

    public override void GetYield(ColonyData colony, string choice)
    {
      if (_dictionary == null)
      {
        _dictionary = new Dictionary<string, WallSegmentName>();
        foreach (var section in _wallSections)
        {
          _dictionary.Add(section.ChoiceName, section.WallSegment);
        }
      }
      if (_defenseType != DamageParameters.Type.none)
      {
        // Adding Defense
        if (colony.Wall.GetSegmentFromName(_dictionary[choice]).currentDefenseType == DamageParameters.Type.none)
        {
          // Wall must not have any defense here for this to work.
          colony.Wall.ApplyDefense(_dictionary[choice], _defenseType);
        }
        else
        {
          Debug.Log($"Cannot install {_defenseType.ToString()} defense on {choice} wall because there already is defense there. Materials wasted.");
        }
      }
      else
      {
        // Clearing Defense
        if (colony.Wall.GetSegmentFromName(_dictionary[choice]).currentDefenseType != DamageParameters.Type.none)
        {
          colony.Wall.ApplyDefense(_dictionary[choice], _defenseType);
        }
        else
        {
          Debug.Log($"Cannot remove defense on {choice} wall because there already is no defense there. Materials wasted.");
        }
      }
    }

    [Serializable]
    public class Entry
    {
      [field: SerializeField] public string ChoiceName { get; private set; }
      [field: SerializeField] public WallSegmentName WallSegment { get; private set; }
    }
  }
}
