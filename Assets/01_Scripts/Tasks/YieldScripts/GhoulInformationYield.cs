using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "GhoulInformationYield", menuName = "Scriptable Objects/Tasks/Yields/Ghoul Information Yield")]
  public class GhoulInformationYield : TaskYield
  {
    [SerializeField] private GhoulSelector _selector;

    public override void GetYield(ColonyData colony, string choice, List<ItemType> consumed)
    {
      Clue clue = _selector.FindClue();
      string innerData = (clue == null) ? clue.dialogBoxInfo : "Nothing of note remaining";
      string clueData = "<indent=0%><align=\"center\">-------------------------------------\n";
      clueData += "<size=40>INVESTIGATION REPORT</size>\n";
      clueData += "-------------------------------------\n";
      clueData += $"<size=30>{innerData}</size>";
      DialogBox.QueueDialogueBox(new DialogueBoxParameters
      (
        GameObject.Find("Canvas").transform,
        clueData,
        new string[] { "Understood" },
        new DialogBox.ButtonEventHandler[] { () => { } }
      ));
    }
  }
}
