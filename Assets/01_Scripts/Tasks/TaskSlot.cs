using UnityEngine.UI;
using UnityEngine;

namespace WallDefense
{
  public class TaskSlot : MonoBehaviour
  {
    [SerializeField] private Image _slotColor;
    [SerializeField] private Image _slotOutline;
    [SerializeField] private DroppableSlotStackable _droppable;
    [SerializeField] private Sprite _meepleOutline;
    [SerializeField] private Sprite _resourceOutline;
    [SerializeField] private TMPro.TextMeshProUGUI _minAmount;
    [SerializeField] private TMPro.TextMeshProUGUI _maxAmount;
    [SerializeField] private Button _autoFillButton;
    [SerializeField] private InventoryData _inventoryData;

    private ItemRequirement _requirement;

    public DroppableSlotStackable Droppable => _droppable;

    public void InitializeSlot(ItemRequirement requirement)
    {
      _requirement = requirement;
      var entry = _requirement.RequirementType;
      _slotColor.sprite = entry.Sprite;
      _slotOutline.sprite = entry.OutlineSprite;
      _droppable.MetadataValidator = entry.Validator;
      _droppable.Initialize(requirement.MinimumMaximumAmount.y);
      _minAmount.text = (requirement.MinimumMaximumAmount.x == 1 && requirement.MinimumMaximumAmount.x == requirement.MinimumMaximumAmount.y) ? "*" : requirement.MinimumMaximumAmount.x.ToString();
      _maxAmount.text = (requirement.MinimumMaximumAmount.x != requirement.MinimumMaximumAmount.y) ? requirement.MinimumMaximumAmount.y.ToString() : "";
      _autoFillButton.gameObject.SetActive(requirement.RequirementType.Metadata != null);
      if (_autoFillButton.gameObject.activeSelf)
      {
        CheckAutoFillAvailable();
      }
    }
    public void CheckAutoFillAvailable()
    {
      Debug.Log(_requirement.IsFulfilled);
      _autoFillButton.interactable = (!_requirement.IsFulfilled &&
          _requirement.Count + _inventoryData.CurrentItems[_requirement.RequirementType] >= _requirement.MinimumMaximumAmount.x);
    }

    public void AutoFill()
    {
      int amountRemaining = _requirement.MinimumMaximumAmount.x - _requirement.Count;
      for (int i = 0; i < amountRemaining; i++)
      {
        _inventoryData.RemoveItem(_requirement.RequirementType, 1);
        GameObject newObject = GameObject.Instantiate(_requirement.RequirementType.Prefab, Droppable.transform);
        var draggable = newObject.GetComponent<DraggableUI>();
        Droppable.CheckAndDrop(draggable);
        Droppable.SetIn(draggable);
      }
      CheckAutoFillAvailable();
    }
  }
}
