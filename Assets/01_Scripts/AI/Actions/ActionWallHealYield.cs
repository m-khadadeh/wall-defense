using UnityEngine;
using WallDefense.AI;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ActionWallHealYield", menuName = "Scriptable Objects/AI/GOAP/Yield/Action Wall Heal Yield")]
  public class ActionWallHealYield : ActionYield
  {
    [SerializeField] private int _amount;
    [SerializeField] private WallSegmentName _segmentName;
    public override void GetYield()
    {
      _colony.Wall.RepairWall(_segmentName, _amount);
    }
  }
}
