using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
  public class WallTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    [SerializeField] private Wall _wall;
    [SerializeField] private WallSegmentName _segmentName;
    private Tooltip _tooltipDisplay;
    [SerializeField] private List<DefenseTypeColor> _defenseTypes;
    private Dictionary<DamageParameters.Type, (string, string)> _colorDictionary;
    public void Awake()
    {
      _colorDictionary = _defenseTypes.ToDictionary(e => e.DamageType, e => (ColorUtility.ToHtmlStringRGB(e.Color), e.DefenseType));
      _tooltipDisplay = GameObject.Find("Tooltip").GetComponent<Tooltip>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
      var wallSegment = _wall.GetSegmentFromName(_segmentName);
      string text = $"Wall {_wall.wallName} {_segmentName.ToString()} segment:\nHP: {wallSegment.health} / {wallSegment.maxhealth}\nCurrent defense type: <color=#{_colorDictionary[wallSegment.currentDefenseType].Item1}>{_colorDictionary[wallSegment.currentDefenseType].Item2}</color>";
      _tooltipDisplay.SetText(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      _tooltipDisplay.SetText("");
    }

    [Serializable]
    public class DefenseTypeColor
    {
      [field: SerializeField] public DamageParameters.Type DamageType { get; private set; }
      [field: SerializeField] public Color Color { get; private set; }
      [field: SerializeField] public string DefenseType { get; private set; }
    }
  }
}
