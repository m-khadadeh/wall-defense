using TMPro;
using UnityEngine;

namespace WallDefense
{
    public class ClueListener : MonoBehaviour
    {
        public GhoulSelector ghoulSelector;
        public TMP_Text clueDescriptions;

        void Awake()
        {
            GhoulSelector.ClueFoundEvent += UpdateText;
            GhoulSelector.ClearClueEvent += ClearText;
        }

        void UpdateText()
        {
            Debug.Log("update text");
            if (ghoulSelector.CurrentGhoul.clueMain.found && ghoulSelector.CurrentGhoul.clueSecondary.found)
            {
                clueDescriptions.text = $"{ghoulSelector.CurrentGhoul.clueMain.description}" + System.Environment.NewLine + $"{ghoulSelector.CurrentGhoul.clueSecondary.description}";
            }
            else if (ghoulSelector.CurrentGhoul.clueMain.found)
            {
                clueDescriptions.text = $"{ghoulSelector.CurrentGhoul.clueMain.description}";
            }
            else if (ghoulSelector.CurrentGhoul.clueSecondary.found)
            {
                clueDescriptions.text = $"{ghoulSelector.CurrentGhoul.clueSecondary.description}";
            }
        }
        void ClearText()
        {
            clueDescriptions.text = "";
        }

        void OnDestroy()
        {
            GhoulSelector.ClueFoundEvent -= UpdateText;
            GhoulSelector.ClearClueEvent -= ClearText;
        }
    }
}
