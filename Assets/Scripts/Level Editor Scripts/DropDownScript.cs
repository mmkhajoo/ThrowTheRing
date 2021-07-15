using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownScript : MonoBehaviour
{

	public Dropdown dropdown;

	public List<string> nameObjects = new List<string>();

	public ResourcesManager resourcesManager;

	public LevelCreator levelCreator;
	// Use this for initialization

	private void Awake()
	{
		for (int i = 0; i < resourcesManager.levelGameObjects.Count; i++)
		{
			nameObjects.Add(resourcesManager.levelGameObjects[i].obj_id);
		}
		dropdown.AddOptions(nameObjects);
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SelectObject(int v)
	{
		Debug.Log(v);
		Debug.Log(nameObjects[v]);
		levelCreator.PassGameObjectToPlaceFromButton(nameObjects[v],0); // just set 0 to not error in code
	}
}
