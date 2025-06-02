using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public static Queue<DialogueBoxParameters> dialogueQueue = new();

        private static void CreateDialogueBox(Transform parent, string prompt, string[] choices, ButtonEventHandler[] eventHandlers)
        {
            if (_prefab == null)
            {
                _prefab = Resources.Load<DialogBoxComponent>("DialogueBoxPrefab");
            }

            //ClearBox();

            DialogBoxComponent newDialogBox = GameObject.Instantiate<DialogBoxComponent>(_prefab, parent);
            newDialogBox.InitializeDialogueBox(prompt, choices, eventHandlers);
            _instance = newDialogBox;
        }

        /// <summary>
        /// Removes the current DialogBoxComponent from the screen.
        /// </summary>
        public static void ClearBox()
        {
            if (_instance != null)
            {
                GameObject.Destroy(_instance.gameObject);

                //if more dialog boxes in queue continue to pop them
                if (dialogueQueue.Count != 0)
                {
                    DequeueDialogueBox();
                }
            }
        }

        /// <summary>
        /// Creates a <c>DialogBoxComponent</c> that shows <c><paramref name="prompt"/></c> and <c><paramref name="choices"/></c> to the player.
        /// It places it as a child to <c><paramref name="parent"/></c> in the heirarchy. It links <c><paramref name="eventHandlers"/></c> to its respective <c><paramref name="choices"/></c>.
        /// </summary>
        public static void QueueDialogueBox(DialogueBoxParameters dialogueBoxParameters)
        {
            dialogueQueue.Enqueue(dialogueBoxParameters);

            //if no active instance, pop next dialogue box
            if (_instance == null)
            {
                DequeueDialogueBox();
            }
        }
        public static void DequeueDialogueBox()
        {
            DialogueBoxParameters _instanceParameters = dialogueQueue.Dequeue();
            CreateDialogueBox(
                _instanceParameters.parent,
                _instanceParameters.prompt,
                _instanceParameters.choices,
                _instanceParameters.eventHandlers
            );
        }
    }
    public class DialogueBoxParameters
    {
        /// <summary>
        /// The parent transform for the DialogBoxComponent, should be on a canvas.
        /// </summary>
        public Transform parent;
        /// <summary>
        /// The prompt / question / information to provide the player.
        /// </summary>
        public string prompt;
        /// <summary>
        /// The choices to provide the player. Each is placed on its own button.
        /// </summary>
        public string[] choices;
        /// <summary>
        /// The respective actions for each choice. Should be the same number of options as <c><paramref name="choices"/></c>.
        /// </summary>
        public DialogBox.ButtonEventHandler[] eventHandlers;
    }
}

