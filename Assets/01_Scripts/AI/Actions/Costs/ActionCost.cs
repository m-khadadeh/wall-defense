using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "ActionCost", menuName = "Scriptable Objects/AI/GOAP/Cost/Basic Action Cost")]
  public class ActionCost : ScriptableObject
  {
    [SerializeField] protected int _baseCost;
    public int Cost { get; protected set; }

    public virtual void Initialize()
    {
      Cost = _baseCost;
    }

    public virtual void UpdateCost()
    {

    }
  }
}
