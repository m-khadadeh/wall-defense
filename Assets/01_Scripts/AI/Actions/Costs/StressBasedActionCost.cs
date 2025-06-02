using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "StressBasedActionCost", menuName = "Scriptable Objects/AI/GOAP/Cost/Stress Based Action Cost")]
  public class StressBasedActionCost : ActionCost
  {
    [SerializeField] private ColonyData _colony;

    public override void Initialize()
    {
      base.Initialize();
      UpdateCost();
    }

    public override void UpdateCost()
    {
      Cost = _colony.GetAIModifiedActionCost(Cost);
    }
  }
}
