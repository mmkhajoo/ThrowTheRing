using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{

	public bool mouseOverUIElement = false;

	public void MouseEnter()
	{
		mouseOverUIElement = true;
	}

	public void MouseExit()
	{
		mouseOverUIElement = false;
	}
}
