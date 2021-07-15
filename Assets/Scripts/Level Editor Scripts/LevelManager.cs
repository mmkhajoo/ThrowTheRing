using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour{

	
	public List<GameObject> objects = new List<GameObject>();
	public List<Level_GameObject> objectsProperties = new List<Level_GameObject>();

	public int CheckSameGameObject(GameObject gameObject)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (gameObject == objects[i])
			{
				return i;
			}
		}

		return -1;
	}

	public void Clear()
	{
		foreach (var VARIABLE in objects)
		{
			Destroy(VARIABLE);
		}
		objects.Clear();
		objectsProperties.Clear();
	}
	
	
	
}
