using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "WallSegmentDefenseTypeSensor", menuName = "Scriptable Objects/AI/GOAP/Sensor/Wall Segment Defense Type Sensor")]
  public class WallSegmentDefenseTypeSensor : Sensor
  {
    [SerializeField] private ColonyData _colony;
    [SerializeField] private WallSegmentName _wallSegment;
    public override void UpdateState()
    {
      _state.StateValue = (int)_colony.Wall.GetSegmentFromName(_wallSegment).currentDefenseType;
    }
  }
}
