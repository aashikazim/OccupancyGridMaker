using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    private GameObject[,] gridObjects;
    
    void Start()
    {
        InitGrid();
    }

    private void InitGrid()
    {
        gridObjects = new GameObject[GlobalVariables.GRID_DIM, GlobalVariables.GRID_DIM];
        for (int i = 0; i < gridObjects.GetLength(0); i++)
        {
            for (int j = 0; j < gridObjects.GetLength(1); j++) 
            {
                gridObjects[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                gridObjects[i, j].transform.position = new Vector3(10 * i, 0, 10 * j);
                gridObjects[i, j].gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }

    }

    private void UpdateGrid()
    {
        for (int i = 0; i < gridObjects.GetLength(0); i++)
        {
            for (int j = 0; j < gridObjects.GetLength(1); j++)
            {
                if (GlobalVariables.occupancyGrid[i, j] > 0.5f)
                {
                    gridObjects[i, j].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                    Debug.Log("Red");
                }
                else if (GlobalVariables.occupancyGrid[i, j] > -0.5f)
                {
                    gridObjects[i, j].gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
                }
                else
                {
                    gridObjects[i, j].gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                    Debug.Log("White");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrid();
    }
}
