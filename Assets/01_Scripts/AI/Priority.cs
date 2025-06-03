using UnityEngine;

namespace WallDefense.AI
{
  public abstract class Priority : ScriptableObject
  {
    public virtual void Initialize()
    {
      
    }
    public abstract int PriorityValue { get; }
    public abstract void UpdatePriority();
  }
}
