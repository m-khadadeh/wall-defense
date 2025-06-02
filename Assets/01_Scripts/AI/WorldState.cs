using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "WorldState", menuName = "Scriptable Objects/AI/GOAP/WorldState")]
  public class WorldState : ScriptableObject
  {
    public int StateValue { get; set; }
  }
}
