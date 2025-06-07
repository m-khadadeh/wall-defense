using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WallDefense
{
    public class Tooltip : MonoBehaviour
    {
        public GameObject tmpContainer;
        public GameObject background;
        public Vector3 offset;
        TMP_Text tmp;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            tmp = tmpContainer.GetComponent<TMP_Text>();
            SetText("");
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Input.mousePosition + offset;
        }
        public void SetText(string text)
        {
            if (text == "")
            {
                tmpContainer.SetActive(false);
                background.SetActive(false);
            }
            else
            {
                tmpContainer.SetActive(true);
                background.SetActive(true);
            }
            tmp.text = text;
        }
    }
}
