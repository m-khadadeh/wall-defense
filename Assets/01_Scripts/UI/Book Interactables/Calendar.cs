using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace WallDefense
{
    public class Calendar : MonoBehaviour
    {
        public DayNightManager dayNightManager;
        public GameObject[] dateLines;
        public TMP_Text dayText;


        public void OnNewDay()
        {
            if (dayNightManager.currentDay % 42 == 0)
            {
                ResetDateLines();
            }
            SetDateLines();
            dayText.text = $"Day: {dayNightManager.currentDay + 1}";
        }

        public void SetDateLines()
        {
            int week = (int)math.floor((dayNightManager.currentDay % 42) / 7);
            int day = dayNightManager.currentDay % 7;
            dateLines[week].transform.localScale = new Vector3((day + 1) / 7.0f, 1, 1);

        }

        public void ResetDateLines()
        {
            foreach (GameObject dateline in dateLines)
            {
                dateline.transform.localScale = new Vector3(0, 1, 1);
            }
        }

    }
}
