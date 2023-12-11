using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
    public static readonly float MAX_VELOCITY = 0.3f;
    public static readonly float MAX_ANGULAR = 2.0f;

    public static readonly float EXPANSION_THRESHOLD = 0.05f;

    public static float velocity_x = 0f;
    public static float angular_x = 0f;

    public static float x = 0f;
    public static float y = 0f;
    public static float theta = 0f;

    public static List<float> ranges = new List<float>();
    public static List<float> range_x = new List<float>();
    public static List<float> range_y = new List<float>();
    public static List<float> gaps_start_x = new List<float>();
    public static List<float> gaps_start_y = new List<float>();
    public static List<float> gaps_end_x = new List<float>();
    public static List<float> gaps_end_y = new List<float>();

    public static float INIT_X = float.NaN;
    public static float INIT_Y = float.NaN;
    public static float INIT_THETA = float.NaN;

    public static float END_POS_X = float.NaN;
    public static float END_POS_Y = float.NaN;
    
    public static readonly float END_POS_DISTANCE = 20.0f;


    public static bool isInitialized = false;


    ///// Occupany grid Map
    public static readonly float WORKSPACE_SIZE = 16f;
    public static readonly float GRID_RESOLUTION = 0.1f;
    public static readonly int GRID_DIM = Mathf.FloorToInt(WORKSPACE_SIZE / GRID_RESOLUTION) + 1;
    public static float[] GRID_POINTS = GenerateGridPoints(WORKSPACE_SIZE, GRID_DIM);


    public static float[,] gx = CreateMeshGridGX();
    public static float[,] gy = CreateMeshGridGY();
    public static float ACTUAL_GRID_RESOLUTION = Mathf.Abs(GRID_POINTS[0] - GRID_POINTS[1]);
    public static float[,] occupancyGrid = CreateOccupancyGrid(); // new float[GRID_DIM, GRID_DIM];


    public static bool isRobotPoseUpdated = false;

    public static float[] GenerateGridPoints(float workspaceSize, int gridDim)
    {
        float[] gridPoints = new float[gridDim];
        float step = workspaceSize / (gridDim - 1);

        for (int i = 0; i < gridDim; i++)
        {
            gridPoints[i] = -workspaceSize / 2 + step * i;
        }
        Debug.Log("gridPoints: " + string.Join(", ", gridPoints));
        return gridPoints;
    }

    private static float[,] CreateMeshGridGX()
    {
        float[,] gx = new float[GRID_DIM, GRID_DIM];

        for (int i = 0; i < GRID_DIM; i++)
        {
            for (int j = 0; j < GRID_DIM; j++)
            {
                gx[i, j] = GRID_POINTS[j];
            }
        }
        Debug.Log("gx[0, 0]: " + gx[0, 0] + ", gx[0, 1]: " + gx[0, 1]);
        return gx;
    }

    private static float[,] CreateMeshGridGY()
    {
        float[,] gy = new float[GRID_DIM, GRID_DIM];

        for (int i = 0; i < GRID_DIM; i++)
        {
            for (int j = 0; j < GRID_DIM; j++)
            {
                gy[i, j] = GRID_POINTS[i];
            }
        }
        
        return gy;
    }
    
    private static float[,] CreateOccupancyGrid()
    {
        float[,] occupancyGrid = new float[GRID_DIM, GRID_DIM];

        // Initialize the occupancyGrid with zeros
        for (int i = 0; i < GRID_DIM; i++)
        {
            for (int j = 0; j < GRID_DIM; j++)
            {
                occupancyGrid[i, j] = 0.0f;
            }
        }
        return occupancyGrid;
    }

}
