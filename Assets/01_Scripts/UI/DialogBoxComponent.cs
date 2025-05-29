using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WallDefense
{
    public class DialogBoxComponent : MonoBehaviour
    {
        [SerializeField] private Button[] buttons;
        [SerializeField] private TextMeshProUGUI[] buttonTexts;
        [SerializeField] private TextMeshProUGUI prompt;

        /// <summary>
        /// Creates a DialogBoxComponent.
        /// </summary>
        /// <param name="boxPrompt"></param>
        /// <param name="options"></param>
        /// <param name="eventHandlers"></param>
        /// <remarks>
        /// Do not use this. Use the <c>DialogBox</c> static class instead.
        /// </remarks>
        public void InitializeDialogueBox(string boxPrompt, string[] options, DialogBox.ButtonEventHandler[] eventHandlers)
        {
            prompt.text = boxPrompt;
            // Buttons should be inactive in the prefab, they will be activated by this function
            for (int i = 0; i < options.Length && i < buttons.Length; i++) // This will only use as many buttons as there are on the dialogue box or less depending on the number of options
            {
                buttons[i].gameObject.SetActive(true);
                buttonTexts[i].text = options[i];

                DialogBox.ButtonEventHandler thisButtonsAction = eventHandlers[i]; // just so no crazy shit happens, im declaring a variable and assigning it to the necessary function
                buttons[i].onClick.AddListener(delegate ()
                {
                    thisButtonsAction?.Invoke();
                    Destroy(gameObject); // Clicking a button should destroy the dialog box
                });
            }
        }
    }
}

