using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CodeStage.AntiCheat.ObscuredTypes;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public UiManager uiManager;

    public GenerateLevel generateLevel;

    public CameraController cameraController;

    // public GenerateLevel generateLevel;

    private bool win;

    public bool game_Start;
    
    public bool isStart;

    public bool out_of_torus;

    public bool GetWin()
    {
        return win;
    }
    void Start()
    {
        out_of_torus = false;
        win = false;
    }

    // Update is called once per frame
    public void Win()
    {
        if (ObscuredPrefs.GetInt("RodCounter") >= ObscuredPrefs.GetInt("RodCount"))
        {
            win = true;
            uiManager.Win();
            generateLevel.StopCoroutine_Torus();
        }
        else
        {
            cameraController.MoveCameraForNextThrow();
            generateLevel.TorusReadyAgain();
        }
    }

    public void Lose()
    {
        generateLevel.StopCoroutine_Torus();
        generateLevel.NextTorus();
        if (game_Start && out_of_torus)
        {
            uiManager.Lose();
        }
        else
        {
            ObscuredPrefs.SetInt("RodCounter", 0);
            generateLevel.ResetRods();
            cameraController.BackToStartPosition();
        }
       
    }

    public void Restart()
    {
        SceneManager.LoadScene("Core");
    }

    public void NextLevel()
    {
        // ObscuredPrefs.SetInt("LevelNumber" , ObscuredPrefs.GetInt("LevelNumber",0)+1);
        SceneManager.LoadScene("Core");
        //WE can use Generate Level Here For Optimize
    }
    
    public void start()
    {
        StartCoroutine(startIE());
    }

    IEnumerator startIE()
    {
        yield return new WaitForSeconds(0.2f);
        isStart = true;
    }
}
