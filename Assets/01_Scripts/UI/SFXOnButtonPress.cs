using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WallDefense
{
  public class SFXOnButtonPress : MonoBehaviour, IPointerClickHandler
  {
    [SerializeField] private string _sfxKey;
    private Selectable _selectable;
    // TODO: This world around buttons that turn themselves off before you can check if they're interactable. TaskUI.RunTask() for ex.
    [SerializeField] private bool _overrideInteractable;
    private Button _buttonForOverride;
    public void Awake()
    {
      _selectable = GetComponent<Selectable>();
      if (_overrideInteractable)
      {
        _buttonForOverride = GetComponent<Button>();
        _buttonForOverride.onClick.AddListener(() => AudioManager.PlaySound(_sfxKey));
      }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if(!_overrideInteractable && _selectable.interactable)
        AudioManager.PlaySound(_sfxKey);
    }
  }
}
