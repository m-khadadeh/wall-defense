using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "ActionGhoulInfoYield", menuName = "Scriptable Objects/AI/GOAP/Yield/Action Ghoul Info Yield")]
  public class ActionGhoulInfoYield : ActionYield
  {
    [SerializeField] private GhoulDiscernmentChalkboard _chalkboard;
    public override void GetYield()
    {
      _chalkboard.OnGhoulResearchComplete();
    }
  }
}
