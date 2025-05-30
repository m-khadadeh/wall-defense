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
      DialogBox.CreateDialogueBox(
        GameObject.Find("Canvas").transform,
        clue != null ? clue.dialogBoxInfo : "There was nothing else",
        new string[] { "Okay" },
        new DialogBox.ButtonEventHandler[] { () => { } }
      );
    }
  }
}
