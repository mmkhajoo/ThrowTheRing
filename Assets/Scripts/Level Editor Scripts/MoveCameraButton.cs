using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCameraButton : MonoBehaviour  , IPointerUpHandler , IPointerDownHandler{

	public bool isDown;
	
	public void OnPointerUp(PointerEventData eventData)
	{
		isDown = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		
		isDown = true;
	}
}
