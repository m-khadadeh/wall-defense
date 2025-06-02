using System.Collections.Generic;
using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "StaticAction", menuName = "Scriptable Objects/AI/GOAP/Action/Static Action")]
  public class StaticAction : Action
  {
    [SerializeField] List<ActionCondition> _conditions;
    [SerializeField] List<ActionYield> _yields;
    [SerializeField] ActionCost _cost;

    public override void ApplyActionState()
    {
      foreach (var actionYield in _yields)
      {
        actionYield.StateResult.ApplyResult();
      }
    }

    public override List<ActionCondition> GetConditions() => _conditions;
    public override ActionCost GetCost() => _cost;

    public override void GetYields()
    {
      foreach (var actionYield in _yields)
      {
        actionYield.GetYield();
      }
    }

    public override void UndoActionState()
    {
      foreach (var actionYield in _yields)
      {
        actionYield.StateResult.UndoResult();
      }
    }
  }
}
