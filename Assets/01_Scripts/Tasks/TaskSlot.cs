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

    public DroppableSlotStackable Droppable => _droppable;

    public void InitializeSlot(ItemRequirement requirement)
    {
      var entry = requirement.RequirementType;
      _slotColor.sprite = entry.Sprite;
      _slotOutline.sprite = entry.OutlineSprite;
      _droppable.MetadataValidator = entry.Validator;
      _droppable.Initialize(requirement.MinimumMaximumAmount.y);
      _minAmount.text = (requirement.MinimumMaximumAmount.x == 1 && requirement.MinimumMaximumAmount.x == requirement.MinimumMaximumAmount.y) ? "*" : requirement.MinimumMaximumAmount.x.ToString();
      _maxAmount.text = (requirement.MinimumMaximumAmount.x != requirement.MinimumMaximumAmount.y) ? requirement.MinimumMaximumAmount.y.ToString() : "";
      //_required.gameObject.SetActive(required);
    }
  }
}
