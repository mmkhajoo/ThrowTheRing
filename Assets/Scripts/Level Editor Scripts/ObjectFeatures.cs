using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Level_Editor_Scripts
{
    public class ObjectFeatures : MonoBehaviour
    {
        public LevelCreator levelCreator;


        //------------------Date Fields Move Feature  -----------------------//

        public GameObject moveFeaturePanel;

        public InputField offset_InputField_X;

        public InputField offset_InputField_Y;

        public InputField time_InputField;

        public Toggle isHorizontal;

        public Toggle isStartAtStart;

        // public string[] objectFeatures;
        //------------------End Data Fields Move Features --------------------------//


        //----------------------Data Fields Rotate Features--------------------------//

        public GameObject RotateFeaturePanel;

        public InputField time_InputField_Rotate;

        //----------------------End Data Fields Rotate Features----------------------//


        // Start is called before the first frame update
        void Start()
        {
            offset_InputField_X.text = "0";
            offset_InputField_Y.text = "0";
        }

        // Update is called once per frame
        void Update()
        {
        }


        public void AddRotateButton()
        {
            float time = float.Parse(time_InputField_Rotate.text);
            AddRotate(time);
        }

        private void AddRotate(float time)
        {
            Rotate rotate;
            if (levelCreator.actualObject.GetComponent<Rotate>() != null)
            {
                rotate = levelCreator.actualObject.GetComponent<Rotate>();
                Destroy(rotate);
            }

            rotate = levelCreator.actualObject.AddComponent<Rotate>();

            var levelGameObject = levelCreator.actualObject.GetComponent<Level_GameObject>();

            rotate.time = time;

            // levelCreator.objProperties.rotateFeature =
            //     new RotateFeature(time);
            // levelCreator.objProperties.isHasRotateFeature = true;
            
            levelGameObject.rotateFeature =
                new RotateFeature(time);
            levelGameObject.isHasRotateFeature = true;
            
        }


        public void MoveButton()
        {
            moveFeaturePanel.SetActive(true);
        }

        public void AddMoveButton()
        {
            float offsetX = float.Parse(offset_InputField_X.text);
            float offsetY = float.Parse(offset_InputField_Y.text);
            float time = float.Parse(time_InputField.text);
            bool isHorizontal = this.isHorizontal.isOn;
            bool isStartAtStart = this.isStartAtStart.isOn;
            AddMove(offsetX, offsetY, time, isHorizontal, isStartAtStart);
        }

        private void AddMove(float offsetX, float offsetY, float time, bool isHorizontal, bool isStartAtStart)
        {
            Move move;
            Level_GameObject levelGameObject;
            if (levelCreator.actualObject.GetComponent<Move>() != null)
            {
                move = levelCreator.actualObject.GetComponent<Move>();
                var startPos = move.firstPos;
                Destroy(move);
                levelCreator.actualObject.transform.position = startPos;
            }

            move = levelCreator.actualObject.AddComponent<Move>();
            levelGameObject = levelCreator.actualObject.GetComponent<Level_GameObject>();

            move.offsetX = offsetX;
            move.offsetY = offsetY;
            move.time = time;
            move.isHorizontal = isHorizontal;
            move.isStartAtStart = isStartAtStart;

            levelGameObject.moveFeature =
                new MoveFeature(offsetX, offsetY, time, isHorizontal, isStartAtStart);
            levelGameObject.isHasMoveFeature = true; 
            // levelCreator.objProperties.moveFeature =
            //     new MoveFeature(offsetX, offsetY, time, isHorizontal, isStartAtStart);
            // levelCreator.objProperties.isHasMoveFeature = true;

            // moveFeaturePanel.SetActive(false);
        }
    }

    [Serializable]
    public class MoveFeature
    {
        public float offsetX; // how much the Object Move From The Start Position

        public float offsetY; // how much the Object Move From The Start Position

        public bool isHorizontal;

        public float time;

        public bool isStartAtStart; // Set Start Position From Bottom Or Top Of The Path;

        public MoveFeature(float offsetX, float offsetY, float time, bool isHorizontal, bool isStartAtStart)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.time = time;
            this.isHorizontal = isHorizontal;
            this.isStartAtStart = isStartAtStart;
        }
    }

    [Serializable]
    public class RotateFeature
    {
        public float time;
        public RotateFeature(float time)
        {
            this.time = time;
        }
    }
}