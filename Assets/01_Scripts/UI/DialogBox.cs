using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
    /// <summary>
    /// A static utility class that generates a <c>DialogBoxComponent</c> that is set as the current instance.
    /// </summary>
    public static class DialogBox
    {
        private static DialogBoxComponent _prefab;
        private static DialogBoxComponent _instance;
        public delegate void ButtonEventHandler();

        /// <summary>
        /// Creates a <c>DialogBoxComponent</c> that shows <c><paramref name="prompt"/></c> and <c><paramref name="choices"/></c> to the player.
        /// It places it as a child to <c><paramref name="parent"/></c> in the heirarchy. It links <c><paramref name="eventHandlers"/></c> to its respective <c><paramref name="choices"/></c>.
        /// </summary>
        /// <param name="parent">The parent transform for the DialogBoxComponent, should be on a canvas.</param>
        /// <param name="prompt">The prompt / question / information to provide the player.</param>
        /// <param name="choices">The choices to provide the player. Each is placed on its own button.</param>
        /// <param name="eventHandlers">The respective actions for each choice. Should be the same number of options as <c><paramref name="choices"/></c>.</param>
        /// <remarks>
        /// If there is already a DialogBoxComponent instance on the screen, it will be replaced by this new one.
        /// </remarks>
        public static void CreateDialogueBox(Transform parent, string prompt, string[] choices, ButtonEventHandler[] eventHandlers)
        {
            if(_prefab == null)
            {
                _prefab = Resources.Load<DialogBoxComponent>("DialogueBoxPrefab");
            }

            ClearBox();

            DialogBoxComponent newDialogBox = GameObject.Instantiate<DialogBoxComponent>(_prefab, parent);
            newDialogBox.InitializeDialogueBox(prompt, choices, eventHandlers);
            _instance = newDialogBox;
        }

        /// <summary>
        /// Removes the current DialogBoxComponent from the screen.
        /// </summary>
        public static void ClearBox()
        {
            if(_instance != null)
            {
                GameObject.Destroy(_instance.gameObject);
            }
        }
    }
}

