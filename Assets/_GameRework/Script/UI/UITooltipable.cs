using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipable : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler {

	public UITooltip tooltip;
	public string tooltipText;

	public void OnPointerEnter(PointerEventData eventData) {
		tooltip.Show(tooltipText);
	}

	public void OnPointerExit(PointerEventData eventData) {
		tooltip.Hide();
	}
}
