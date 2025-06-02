using UnityEngine;

namespace WallDefense.AI
{
  public abstract class ActionYield : ScriptableObject
  {
    [SerializeField] protected ColonyData _colony;
    [field: SerializeField] public ActionStateResult StateResult { get; private set; }
    public abstract void GetYield();
  }
}
