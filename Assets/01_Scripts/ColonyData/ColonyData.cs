using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ColonyData", menuName = "Scriptable Objects/ColonyData")]
  public class ColonyData : ScriptableObject
  {
    [field: SerializeField] public InventoryData Inventory { get; private set; }
    [field: SerializeField] public Wall Wall { get; private set; }
  }
}
