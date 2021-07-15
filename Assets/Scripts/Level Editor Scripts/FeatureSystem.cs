using System.Collections;
using System.Collections.Generic;
using Level_Editor_Scripts;
using UnityEngine;

public class FeatureSystem
{
    // Start is called before the first frame update
    public void AddFeatures(GameObject gameObject, SaveableLevelObject levelObject)
    {
        if (levelObject.isHasMoveFeature)
        {
            Move tempMove = gameObject.AddComponent<Move>();

            tempMove.offsetX = levelObject.moveFeature.offsetX;
            tempMove.offsetY = levelObject.moveFeature.offsetY;
            tempMove.time = levelObject.moveFeature.time;
            tempMove.isHorizontal = levelObject.moveFeature.isHorizontal;
            tempMove.isStartAtStart = levelObject.moveFeature.isStartAtStart;
        }

        if (levelObject.isHasRotateFeature)
        {
            Rotate tempRotate = gameObject.AddComponent<Rotate>();

            tempRotate.time = levelObject.rotateFeature.time;
        }
    }
    
    public void AddFeaturesInLevelEditor(GameObject gameObject,SaveableLevelObject levelObject)
    {
        Level_GameObject levelGameObject = gameObject.GetComponent<Level_GameObject>();
        
        if (levelObject.isHasMoveFeature)
        {
            Move tempMove = gameObject.AddComponent<Move>();
            
            tempMove.offsetX = levelObject.moveFeature.offsetX;
            tempMove.offsetY = levelObject.moveFeature.offsetY;
            tempMove.time = levelObject.moveFeature.time;
            tempMove.isHorizontal = levelObject.moveFeature.isHorizontal;
            tempMove.isStartAtStart = levelObject.moveFeature.isStartAtStart;

            levelGameObject.isHasMoveFeature = true;
            levelGameObject.moveFeature = new MoveFeature(levelObject.moveFeature.offsetX,
                levelObject.moveFeature.offsetY,levelObject.moveFeature.time,
                levelObject.moveFeature.isHorizontal,levelObject.moveFeature.isStartAtStart);
        }
        
        if (levelObject.isHasRotateFeature)
        {
            Rotate tempRotate = gameObject.AddComponent<Rotate>();

            levelGameObject.isHasRotateFeature = true;
            levelGameObject.rotateFeature = new RotateFeature(levelObject.rotateFeature.time);
            
            tempRotate.time = levelObject.rotateFeature.time;
        }
    }
}
