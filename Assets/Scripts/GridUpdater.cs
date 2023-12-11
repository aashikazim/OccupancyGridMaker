using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalVariables.isRobotPoseUpdated)
        {
            UtilityFunctions.UpdateFreeSpace();
            UtilityFunctions.UpdateObstacles();
            GlobalVariables.isRobotPoseUpdated = false;
        }
    }
}
