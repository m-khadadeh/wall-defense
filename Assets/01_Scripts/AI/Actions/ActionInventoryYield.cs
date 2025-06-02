using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "ActionInventoryYield", menuName = "Scriptable Objects/AI/GOAP/Yield/Action Inventory Yield")]
  public class ActionInventoryYield : ActionYield
  {
    [SerializeField] ActionStateResult.StateChange _addOrSubtract;
    [SerializeField] ItemType _item;
    [SerializeField] int _amount;
    public override void GetYield()
    {
      switch (_addOrSubtract)
      {
        case ActionStateResult.StateChange.Add:
          _colony.Inventory.AddItem(_item, _amount);
          break;
        case ActionStateResult.StateChange.Subtract:
          _colony.Inventory.RemoveItem(_item, _amount);
          break;
        default:
          throw new System.Exception("Unavailable inventory yield action");
      }
    }
  }
}
