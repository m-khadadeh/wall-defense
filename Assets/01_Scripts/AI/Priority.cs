using UnityEngine;

namespace WallDefense.AI
{
  public abstract class Priority : ScriptableObject
  {
    [SerializeField] protected int _startingPriority;
    [SerializeField] protected int _priority;
    [SerializeField] protected WorldState[] _worldStates;
    public int PriorityValue => _priority;

    public abstract void UpdatePriority();
  }
}
