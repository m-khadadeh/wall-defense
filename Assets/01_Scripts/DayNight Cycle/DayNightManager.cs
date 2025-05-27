using UnityEngine;

namespace WallDefense
{
    public class DayNightManager : MonoBehaviour
    {
        [SerializeField]
        int currentDay, currentHour;
        /// <summary>
        /// Start of attack window
        /// </summary>
        public int nightStart = 23;
        /// <summary>
        /// End of attack window
        /// </summary>
        public int dayStart = 6;
        public int nextTaskCompletionHour = 0;

        public void AdvanceHour()
        {
            OnBeforeHour();
            currentHour = (currentHour + 1) % 24;
            if (currentHour == 0)
            {
                OnNewDay();
            }
            OnAfterHour();
        }
        public void OnBeforeHour()
        {

        }
        public void OnAfterHour()
        {

        }
        public void OnNewDay()
        {

        }
        public void AdvanceToTaskComplete()
        {
            while (currentHour != nextTaskCompletionHour)
            {
                AdvanceHour();
            }
        }
    }
}
