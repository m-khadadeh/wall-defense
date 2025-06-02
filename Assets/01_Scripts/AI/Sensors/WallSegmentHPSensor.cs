using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "WallSegmentHPSensor", menuName = "Scriptable Objects/AI/GOAP/Sensor/Wall Segment HP Sensor")]
  public class WallSegmentHPSensor : Sensor
  {
    [SerializeField] private Wall _wall;
    [SerializeField] private WallSegmentName _wallSegment;
    public override void UpdateState()
    {
      _state.StateValue = _wall.GetSegmentFromName(_wallSegment).health;
    }
  }
}
