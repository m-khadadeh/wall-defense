using UnityEngine;

namespace WallDefense
{
    public class uitest : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void TestDialogueBox()
        {
            DialogBox.QueueDialogueBox(new DialogueBoxParameters
            {
                parent = transform,
                prompt = "Hey, test out this dialogue box!",
                choices = new string[] { "Sure!", "No :[" },
                eventHandlers = new DialogBox.ButtonEventHandler[] {
                    () =>
                    {
                        Debug.Log("Thanks!");
                    },
                    () =>
                    {
                        Debug.Log("Fuck you!");
                    }
               }
            }
            );
            DialogBox.QueueDialogueBox(new DialogueBoxParameters
            {
                parent = transform,
                prompt = "anotha one",
                choices = new string[] { "Sure!", "No :[" },
                eventHandlers = new DialogBox.ButtonEventHandler[] {
                    () =>
                    {
                        Debug.Log("yay!");
                    },
                    () =>
                    {
                        Debug.Log("wtfbro!");
                    }
               }
            }
            );
        }
    }
}
