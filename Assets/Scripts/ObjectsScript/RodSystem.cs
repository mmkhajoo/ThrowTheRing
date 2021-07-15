using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodSystem : MonoBehaviour
{


    public GameObject rod;

    public GameObject bottomRod;

    public GameObject ground;
    // Start is called before the first frame update

    public void Reset()
    {
        rod.SetActive(true);
        bottomRod.SetActive(true);
        ground.SetActive(false);
    }
}
