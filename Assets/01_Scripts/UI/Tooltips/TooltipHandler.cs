using UnityEngine;
using UnityEngine.EventSystems;

namespace WallDefense
{
    public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Tooltip tooltipDisplay;
        public string text;
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipDisplay.tmpContainer.SetActive(true);
            tooltipDisplay.SetText(text);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipDisplay.SetText("");
            tooltipDisplay.tmpContainer.SetActive(false);
        }
    }
}
