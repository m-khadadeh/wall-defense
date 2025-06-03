using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "ActionLiarYield", menuName = "Scriptable Objects/AI/GOAP/Yield/Action Liar Yield")]
  public class ActionLiarYield : ActionYield
  {
    public override void GetYield()
    {
      // AND WE FUCKING LIE ABOUT IT
    }
  }
}
