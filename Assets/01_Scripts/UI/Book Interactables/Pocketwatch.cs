using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace WallDefense
{
    public class Pocketwatch : MonoBehaviour
    {
        public GameObject hourHand;
        public GameObject[] slices;
        public DayNightManager dayNightManager;
        public Calendar calendar;

        void Start()
        {
            OnAdvanceHour();
        }

        public void OnAdvanceHour()
        {
            hourHand.transform.rotation = Quaternion.Euler(0, 0, -(15 * dayNightManager.currentHour));
            if (dayNightManager.currentHour == 0)
            {
                ClearSlices();
                calendar.OnNewDay();
            }
            ColorSlices();
        }

        public void ColorSlices()
        {
            for (int i = 0; i < slices.Length; i++)
            {
                if (i < dayNightManager.currentHour)
                {
                    slices[i].SetActive(true);
                }
            }
        }
        public void ClearSlices()
        {
            foreach (GameObject slice in slices)
            {
                slice.SetActive(false);
            }
        }
    }
}
