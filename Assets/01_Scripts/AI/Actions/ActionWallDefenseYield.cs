using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "ActionWallDefenseYield", menuName = "Scriptable Objects/AI/GOAP/Yield/Action Wall Defense Yield")]
  public class ActionWallDefenseYield : ActionYield
  {
    [SerializeField] private WallSegmentName _wallSegment;
    [SerializeField] private DamageParameters.Type _damageType;

    public override void GetYield()
    {
      _colony.Wall.ApplyDefense(_wallSegment, _damageType);
    }
  }
}
