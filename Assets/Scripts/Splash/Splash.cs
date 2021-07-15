using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartGameForFirstTime();
		SceneManager.LoadScene("Core");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGameForFirstTime()
	{
		if (!ObscuredPrefs.HasKey("StartGameForFirstTime"))
		{
			ObscuredPrefs.SetInt("LevelNumber",1);
			ObscuredPrefs.SetInt("StartGameForFirstTime",1);
		}
	}
	
}
