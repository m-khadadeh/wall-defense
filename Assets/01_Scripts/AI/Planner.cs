using System.Collections.Generic;
using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "Planner", menuName = "Scriptable Objects/AI/GOAP/Planner")]
  public class Planner : ScriptableObject
  {
    [SerializeField] private List<Sensor> _sensors;
    [SerializeField] private List<Goal> _goals;
    [SerializeField] private List<HourCountdownActionCost> _countdownCosts;
    [SerializeField] private List<GhoulTimeBasedPriority> _hourPriorities;

    public void Initialize()
    {
      foreach (var goal in _goals)
      {
        goal.Initialize();
      }
      foreach (var sensor in _sensors)
      {
        sensor.UpdateState();
      }
    }

    public void OnAfterHour(int hour)
    {      
      foreach (var sensor in _sensors)
      {
        sensor.UpdateState();
      }
      foreach (var priority in _hourPriorities)
      {
        priority.OnHour(hour);
      }
      foreach (var cost in _countdownCosts)
      {
        cost.OnHour(hour);
      }
      foreach (var goal in _goals)
      {
        goal.UpdateCost();
        goal.UpdatePriority();
      }
    }
  }
}
