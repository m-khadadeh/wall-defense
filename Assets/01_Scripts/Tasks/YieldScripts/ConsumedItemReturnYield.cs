using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ConsumedItemReturnYield", menuName = "Scriptable Objects/Tasks/Yields/Consumed Item Return Yield")]
  public class ConsumedItemReturnYield : TaskYield
  {
    [SerializeField] private MetadataValidator _itemValidator;
    public override void GetYield(ColonyData colony, string choice, List<ItemType> consumed)
    {
      foreach (var item in consumed)
      {
        if (_itemValidator.Validate(item.Metadata))
        {
          colony.Inventory.AddItem(item, 1);
        }
      }
    }
  }
}
