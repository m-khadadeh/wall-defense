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
        public UnityEvent onBeforeHour, onAfterHour, onNewDay, onNightHour;
        public bool triggerHourAdvance = false, advanceToTaskComplete = false;

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
            if (advanceToTaskComplete)
            {
                AdvanceToTaskComplete();
                advanceToTaskComplete = false;
            }
        }
        public void AdvanceHour()
        {
            onBeforeHour.Invoke();
            currentHour = (currentHour + 1) % 24;
            if (currentHour == dayStart)
            {
                onNewDay.Invoke();
            }
            if (currentHour >= nightStart || currentHour < dayStart)
            {
                onNightHour.Invoke();
            }
            onAfterHour.Invoke();
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
