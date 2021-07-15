using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New LeveObject", menuName = "LevelObject/New LevelObject")]


public class SaveLevelObject : ScriptableObject
{
   public LevelObject levelObject;
}
[Serializable]
public class LevelObject
{
   public LevelObject()
   {
//      move = null;
   }

   public LevelObject(Moveable move)
   {
      this.move.start = move.start;
      this.move.end = move.end;
      this.move.speed = move.speed;
   }
   
   public string objectName; // Object Name

   public GameObject gameObject; // The GameObject We will Instantiate in Scene

   public Moveable move;
   
}

[Serializable]
public class Moveable
{
   public Moveable()
   {
      
   }

   public Moveable(Vector2 start , Vector2 end , float speed)
   {
      this.start = start;
      this.end = end;
      this.speed = speed;
//      this.image = image;
   }
   public Vector2 start;

   public Vector2 end;
   
   public float speed;

   public Sprite image;
}
//[System.Serializable]
//public class Data
//{
//   public string[] name;
//
//   public int count;

