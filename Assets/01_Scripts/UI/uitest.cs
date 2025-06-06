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
            (
                transform,
                "Hey, test out this dialogue box!",
                new string[] { "Sure!", "No :[" },
                new DialogBox.ButtonEventHandler[] {
                    () =>
                    {
                        Debug.Log("Thanks!");
                    },
                    () =>
                    {
                        Debug.Log("Fuck you!");
                    }
               }
            )
            );
            DialogBox.QueueDialogueBox(new DialogueBoxParameters
            (
                transform,
                "anotha one",
                new string[] { "Sure!", "No :[" },
                new DialogBox.ButtonEventHandler[] {
                    () =>
                    {
                        Debug.Log("yay!");
                    },
                    () =>
                    {
                        Debug.Log("wtfbro!");
                    }
               }
            )
            );
        }
    }
}
