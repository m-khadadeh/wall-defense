using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "WallDefenseYield", menuName = "Scriptable Objects/Tasks/Yields/Wall Defense Yield")]
  public class WallDefenseYield : WallSegmentYield
  {
    [SerializeField] private DamageParameters.Type _defenseType;

    public override void GetYield(ColonyData colony, string choice, List<ItemType> consumed)
    {
      InitializeDictionaryIfNot();
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
          string memo = "<indent=0%><align=\"center\">-------------------------------------\n";
          memo += "<size=40>DEFENSE INSTALLATION ISSUE</size>\n";
          memo += "-------------------------------------\n";
          memo += $"<size=26>Could not install {_defenseType.ToString()} on {choice} wall because there is already defense installed there.</size>\n";
          memo += $"<size=26>Materials wasted.\n";
          DialogBox.QueueDialogueBox(new DialogueBoxParameters
            (
              GameObject.Find("Canvas").transform,
              memo,
              new string[] { "Understood" },
              new DialogBox.ButtonEventHandler[] { () => { } }
            )
          );
        }
      }
      else
      {
        string memo = "<indent=0%><align=\"center\">-------------------------------------\n";
        memo += "<size=40>DEFENSE INSTALLATION ISSUE</size>\n";
        memo += "-------------------------------------\n";
        memo += $"<size=26>Could not install {_defenseType.ToString()} on {choice} wall because there is already defense installed there.</size>\n";
        memo += $"<size=26>Materials wasted.\n";
        // Clearing Defense
        if (colony.Wall.GetSegmentFromName(_dictionary[choice]).currentDefenseType != DamageParameters.Type.none)
        {
          colony.Wall.ApplyDefense(_dictionary[choice], _defenseType);
        }
        else
        {
          DialogBox.QueueDialogueBox(new DialogueBoxParameters
            (
              GameObject.Find("Canvas").transform,
              $"Cannot remove defense on {choice} wall because there already is no defense there.",
              new string[] { "Understood" },
              new DialogBox.ButtonEventHandler[] { () => { } }
            )
          );
        }
      }
    }
  }
}
