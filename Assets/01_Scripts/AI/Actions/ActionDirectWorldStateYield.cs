using UnityEngine;

namespace WallDefense.AI
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
