using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "InventorySensor", menuName = "Scriptable Objects/AI/GOAP/Sensor/Inventory Sensor")]
  public class InventorySensor : Sensor
  {
    [SerializeField] private InventoryData _inventory;
    [SerializeField] private ItemType _item;
    public override void UpdateState()
    {
      _state.StateValue = _inventory.CurrentItems[_item];
    }
  }
}
