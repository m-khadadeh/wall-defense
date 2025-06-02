using UnityEngine;

namespace WallDefense.AI
{
  public abstract class Sensor : ScriptableObject
  {
    [SerializeField] protected WorldState _state;
    public abstract void UpdateState();
  }
}
