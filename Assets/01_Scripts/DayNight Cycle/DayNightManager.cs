using System;
using UnityEngine;
using UnityEngine.Events;

namespace WallDefense
{
    public class DayNightManager : MonoBehaviour
    {
        public int currentDay, currentHour;
        /// <summary>
        /// Start of attack window
        /// </summary>
        public int nightStart = 23;
        /// <summary>
        /// End of attack window
        /// </summary>
        public int dayStart = 6;
        public int nextTaskCompletionHour = 0;
        public UnityEvent onNewDay;
        public UnityEvent<int> onBeforeHour, onAfterHour, onNightHour;
		public bool triggerHourAdvance = false;

        void Start()
        {
            AdvanceHour();
        }
        void Update()
        {
            if (triggerHourAdvance)
            {
                AdvanceHour();
                triggerHourAdvance = false;
            }
        }
        public void AdvanceHour()
        {
            onBeforeHour.Invoke(currentHour);
            currentHour = (currentHour + 1) % 24;
            if (currentHour == dayStart)
            {
                onNewDay.Invoke();
            }
            if (currentHour >= nightStart || currentHour < dayStart)
            {
                onNightHour.Invoke(currentHour);
            }
            onAfterHour.Invoke(currentHour);
        }

        public void AdvanceToTaskComplete(TaskManager taskManager)
        {
            int hoursToAdvance = taskManager.GetShortestTimeToTaskCompletion();
            Debug.Log(hoursToAdvance);
            for (int i = 0; i < hoursToAdvance; i++)
            {
                AdvanceHour();
            }
        }
        public void LoadData(DayNightData dayNightData)
        {
            currentDay = dayNightData.currentDay;
            currentHour = dayNightData.currentHour;
        }
    }
}
