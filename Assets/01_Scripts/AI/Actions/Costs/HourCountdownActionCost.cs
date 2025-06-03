using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "HourCountdownActionCost", menuName = "Scriptable Objects/AI/GOAP/Cost/Hour Countdown Action Cost")]
  public class HourCountdownActionCost : ActionCost
  {
    [SerializeField] private int _countdownToHour;
    private int _currentHour;

    public override void Initialize()
    {
      _currentHour = _baseCost;
      UpdateCost();
    }

    public override void UpdateCost()
    {
      if (_currentHour < _countdownToHour)
      {
        Cost = _countdownToHour - _currentHour;
      }
      else
      {
        Cost = 24 - (_currentHour - _countdownToHour);
      }
    }

    public void OnHour(int hour)
    {
      _currentHour = hour;
      UpdateCost();
    }
  }
}
