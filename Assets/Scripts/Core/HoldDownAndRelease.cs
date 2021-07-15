using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldDownAndRelease : MonoBehaviour, IPointerUpHandler , IPointerDownHandler
{
   
    public bool isPressed;

    public InterfaceManager interfaceManager;

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(MouseExit());
    }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("BrakeScript Called");
            interfaceManager.MouseEnter();
        }

        
        private IEnumerator MouseExit()
        {
            yield return new WaitForSeconds(0.1f);
            interfaceManager.MouseExit();
        }
    
}
