using UnityEngine;

namespace WallDefense
{
    public class ToggleTab : MonoBehaviour
    {
        public bool on;
        public GameObject redTab, greenTab;
        void Start()
        {
            on = true;
            Toggle();
        }

        public void ResetTab()
        {
            on = true;
            Toggle();
        }

        public void Toggle()
        {
            on = !on;
            redTab.SetActive(!on);
            greenTab.SetActive(on);
        }
    }
}
