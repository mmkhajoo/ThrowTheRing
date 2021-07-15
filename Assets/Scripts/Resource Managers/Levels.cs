using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Levels", menuName = "Level/New Level")]
[Serializable]
public class Levels : ScriptableObject
{
    public List<SaveableLevel_SaveLoad> levels;
    // Start is called before the first frame update
}
