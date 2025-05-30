using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "ItemGiftYield", menuName = "Scriptable Objects/Tasks/Yields/Item Gift Yield")]
  public class ConsumedItemGiftYield : TaskYield
  {
    [SerializeField] MetadataValidator _giftValidator;
    [SerializeField] List<Entry> _giftableColonies;
    private Dictionary<string, ColonyData> _colonies;

    public override void GetYield(ColonyData colony, string choice, List<ItemType> consumed)
    {
      if (_colonies == null)
      {
        Initialize();
      }
      List<ItemType> gifts = new List<ItemType>();
      foreach (var item in consumed)
      {
        if (_giftValidator.Validate(item.Metadata))
        {
          gifts.Add(item);
        }
      }
      _colonies[choice].ReceiveGift(gifts);
    }

    private void Initialize()
    {
      _colonies = new Dictionary<string, ColonyData>();
      foreach (var colony in _giftableColonies)
      {
        _colonies.Add(colony.colonyKey, colony.colonyValue);
      }
    }

    [Serializable]
    public class Entry
    {
      [field: SerializeField] public string colonyKey;
      [field: SerializeField] public ColonyData colonyValue;
    }
  }
}
