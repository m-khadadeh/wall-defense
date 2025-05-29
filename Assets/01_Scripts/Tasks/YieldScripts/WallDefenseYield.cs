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
          DialogBox.CreateDialogueBox(
            GameObject.Find("Canvas").transform,
            $"Cannot install {_defenseType.ToString()} defense on {choice} wall because there already is defense there. Materials wasted.",
            new string[] { "Okay" },
            new DialogBox.ButtonEventHandler[] { () => { }}
          );
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
          DialogBox.CreateDialogueBox(
            GameObject.Find("Canvas").transform,
            $"Cannot remove defense on {choice} wall because there already is no defense there. Materials wasted.",
            new string[] { "Okay" },
            new DialogBox.ButtonEventHandler[] { () => { }}
          );
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
