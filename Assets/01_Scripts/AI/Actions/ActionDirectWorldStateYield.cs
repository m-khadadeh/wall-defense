using UnityEngine;
using WallDefense.AI;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ActionDirectWorldStateYield", menuName = "Scriptable Objects/AI/GOAP/Yield/Action Direct World State Yield")]
  public class ActionDirectWorldStateYield : ActionYield
  {
    public override void GetYield()
    {
      StateResult.ApplyResult();
    }
  }
}
