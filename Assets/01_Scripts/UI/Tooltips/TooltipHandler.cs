using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
    public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Tooltip tooltipDisplay;
        public string text;

        public void Awake()
        {
            tooltipDisplay = GameObject.Find("Tooltip").GetComponent<Tooltip>();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipDisplay.SetText(text);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipDisplay.SetText("");
        }
    }
}
