using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject winPanel;

    public GameObject losePanel;

    public Button test;
    void Start()
    {
        
    }

    public void Lose()
    {
        losePanel.SetActive(true);
    }

    public void Win()
    {
        winPanel.SetActive(true);
    }
    // Update is called once per frame
    
}
