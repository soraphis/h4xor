using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapTransparency : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField] private Image image;

	void OnValidate() { image = GetComponent<Image>(); }

	void Start() {
		image.color = new Color(0, 0, 0, 0.8f);
	}
	
	public void OnPointerEnter(PointerEventData eventData) {
		image.color = new Color(0, 0, 0, 0f);
		
	}

	public void OnPointerExit(PointerEventData eventData) {
		image.color = new Color(0, 0, 0, 1);
	}
}
