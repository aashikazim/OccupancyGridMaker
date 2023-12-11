using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class UtilityFunctions
{
    
    public static void InitEndPoints()
    {
        GlobalVariables.INIT_X = GlobalVariables.x;
        GlobalVariables.INIT_Y = GlobalVariables.y;
        GlobalVariables.INIT_THETA = GlobalVariables.theta;

        GlobalVariables.END_POS_X = GlobalVariables.INIT_X + GlobalVariables.END_POS_DISTANCE * Mathf.Cos(GlobalVariables.INIT_THETA);
        GlobalVariables.END_POS_Y = GlobalVariables.INIT_Y + GlobalVariables.END_POS_DISTANCE * Mathf.Sin(GlobalVariables.INIT_THETA);
    }


    public static void UpdateRobotPose(RobotPose robotPose)
    {
        GlobalVariables.x = robotPose.x;
        GlobalVariables.y = robotPose.y;
        GlobalVariables.theta = robotPose.theta;
        GlobalVariables.ranges = robotPose.range;
        if (GlobalVariables.isInitialized == false)
        {
            GlobalVariables.isInitialized = true;
            InitEndPoints();
        }
        //Debug.Log("Updated: ");
        //Debug.Log(robotPose);
        //UpdateFreeSpace();
        //UpdateObstacles();

        GlobalVariables.isRobotPoseUpdated = true;
    }

    public static int[] GetGridIdx(float rx, float ry)
    {
        rx = rx - GlobalVariables.INIT_X;
        ry = ry - GlobalVariables.INIT_Y;

        int[] gridIdx = null;
        
        if ((Mathf.Abs(rx) >= GlobalVariables.WORKSPACE_SIZE / 2) || (Mathf.Abs(ry) >= GlobalVariables.WORKSPACE_SIZE / 2))
        {
            return gridIdx;
        }

        gridIdx = new int[2];
        //Debug.Log("Initialized");
        gridIdx[0] = (GlobalVariables.GRID_DIM + 1) / 2 + (int)((rx + GlobalVariables.ACTUAL_GRID_RESOLUTION / 2) / GlobalVariables.ACTUAL_GRID_RESOLUTION) - 1;
        gridIdx[1] = (GlobalVariables.GRID_DIM + 1) / 2 + (int)((ry + GlobalVariables.ACTUAL_GRID_RESOLUTION / 2) / GlobalVariables.ACTUAL_GRID_RESOLUTION) - 1;

        //Debug.Log("Grid Idx: " + gridIdx[0] + ", " + gridIdx[1]);
        //Debug.Log("Grid Dim: " + GlobalVariables.GRID_DIM);
        //Debug.Log("Grid Resolution: " + GlobalVariables.ACTUAL_GRID_RESOLUTION);
        //Debug.Log("rx: " + rx);
        //Debug.Log("ry: " + ry);
        return gridIdx;
    }

    public static void UpdateFreeSpace()
    {
        for (int i = 0; i < GlobalVariables.ranges.Count; i++)
        {
            //Scanning Within 1 Meter Range
            float d = GlobalVariables.ranges[i];
            float angle = GlobalVariables.theta + i * 0.5f * Mathf.PI / 180;
            if (d > 10.0f)
            {
                d = 10.0f;
            }

            //Calculating Start Point of Free Space
            int[] pfs = GetGridIdx(GlobalVariables.x, GlobalVariables.y);

            if (pfs == null)
            {
                continue;
            }

            //Calculating End Point of Free Space
            float fs_end_x = GlobalVariables.x + d * Mathf.Cos(angle);
            float fs_end_y = GlobalVariables.y + d * Mathf.Sin(angle);

            int[] pfe = GetGridIdx(fs_end_x, fs_end_y);

            if (pfe == null)
            {
                continue;
            }

            //Updating Free Space in Occupancy Grid
            Tuple<List<int>, List<int>> pair = BresenhamLine(pfs[0], pfs[1], pfe[0], pfe[1]);
            for (int j = 0; j < pair.Item1.Count - 1; j++)
            {
                int x = pair.Item1[j];
                int y = pair.Item2[j];
                GlobalVariables.occupancyGrid[x, y] -= 0.02f;
                GlobalVariables.occupancyGrid[x, y] = Mathf.Max(GlobalVariables.occupancyGrid[x, y], -1.0f);
            }
            //Debug.Log("Updated Free Space");
        }
    }
    public static void UpdateObstacles()
    {
        float obs_real_x = 0.0f;
        float obs_real_y = 0.0f;

        List<int> obs_x = new List<int>();
        List<int> obs_y = new List<int>();


        List<Tuple<int, int>> obs_filled = new List<Tuple<int, int>>();
        //bool expansion = true;
        //float expansionThreshold = GlobalVariables.EXPANSION_THRESHOLD;
        //int obs_connect = 0;
        //int pre_grid_x = 0;
        //int pre_grid_y = 0;

        //Scanning Within 1 Meter Range and Adding Obstacle to Obstacle List 

        for (int i = 0; i < GlobalVariables.ranges.Count; i++)
        {
            float d = GlobalVariables.ranges[i];
            float angle = GlobalVariables.theta + i * 0.5f * Mathf.PI / 180;
            if (d <= 10.0f)
            {
                obs_real_x = GlobalVariables.x + d * Mathf.Cos(angle);
                obs_real_y = GlobalVariables.y + d * Mathf.Sin(angle);
                GlobalVariables.range_x.Add(obs_real_x);
                GlobalVariables.range_y.Add(obs_real_y);
                
                int[] obs_grid_xy = GetGridIdx(obs_real_x, obs_real_y);
                if (obs_grid_xy == null)
                {
                    continue;
                }
                int x = obs_grid_xy[0];
                int y = obs_grid_xy[1];
                //Debug.Log("x: " + x + " y: " + y);
                GlobalVariables.occupancyGrid[x, y] += 0.05f;
                GlobalVariables.occupancyGrid[x, y] = Mathf.Min(GlobalVariables.occupancyGrid[x, y], 1.0f);

                //if (MathF.Abs(obs_grid_xy[0] - pre_grid_x) + MathF.Abs(obs_grid_xy[1] - pre_grid_y) == 0)
                //{
                //    continue;
                //}
                //pre_grid_x = obs_grid_xy[0];
                //pre_grid_y = obs_grid_xy[1];
                //obs_x.Add(obs_grid_xy[0]);
                //obs_y.Add(obs_grid_xy[1]);

            }
        }

        // Computing Obstacles
        //for (int i = 0; i < obs_x.Count - 1; i++)
        //{
        //    int px1 = obs_x[i];
        //    int py1 = obs_y[i];
        //    int px2 = obs_x[i + 1];
        //    int py2 = obs_y[i + 1];

        //    if ((MathF.Abs(px1 - px2) + MathF.Abs(py1 - py2)) < (int)(obs_connect / GlobalVariables.GRID_RESOLUTION) + 1)
        //    {
        //        Tuple<List<int>, List<int>> pair = BresenhamLine(px1, py1, px2, py2);
        //        if (pair.Item1.Count > 0)  
        //        {
        //            for (int j = 0; j < pair.Item1.Count - 1; j++)
        //            {
        //                if (pair.Item1[j] != int.MinValue && pair.Item2[j] != int.MinValue)
        //                {
        //                    obs_filled.Add(new Tuple<int, int>(pair.Item1[j], pair.Item2[j]));
        //                }
        //                //obs_filled.Add(new Tuple<int, int>(pair.Item1[j], pair.Item2[j]));
        //            }
        //        }
        //        else
        //        {
        //            if (px1 != int.MinValue && py1 != int.MinValue)
        //            {
        //                obs_filled.Add(new Tuple<int, int>(px1, py1));
        //            }
                    
        //        }
        //    }

        //}
        
        //if (obs_x.Count > 0)
        //{
        //    if (obs_x[obs_x.Count - 1] != int.MinValue && obs_y[obs_y.Count - 1] != int.MinValue)
        //    {
        //        obs_filled.Add(new Tuple<int, int>(obs_x[obs_x.Count - 1], obs_y[obs_y.Count - 1]));
        //    }
        //    //obs_filled.Add(new Tuple<int, int>(obs_x[obs_x.Count - 1], obs_y[obs_y.Count - 1]));
        //}

        //// updating free spaces in grid
        //for (int i = 0; i < obs_filled.Count; i++)
        //{
        //    int x = obs_filled[i].Item1;
        //    int y = obs_filled[i].Item2;
        //    //Debug.Log("x: " + x + " y: " + y);
        //    GlobalVariables.occupancyGrid[x, y] += 0.05f;
        //    GlobalVariables.occupancyGrid[x, y] = Mathf.Min(GlobalVariables.occupancyGrid[x, y], 1.0f);
        //}

    }


    public static Tuple<List<int>, List<int>> BresenhamLine(int x0, int y0, int x1, int y1)
    {
        int dx = x1 - x0;
        int dy = y1 - y0;
        int xsign = 1;
        int ysign = 1;

        if (dx < 0)
        {
            xsign = -1;
        }

        if (dy < 0)
        {
            ysign = -1;
        }
        
        dx = Mathf.Abs(dx);
        dy = Mathf.Abs(dy);
        
        int xx, xy, yx, yy;

        if (dx > dy)
        {
            xx = xsign;
            xy = 0;
            yx = 0;
            yy = ysign;
        }
        else
        {
            int temp = dx;
            dx = dy;
            dy = temp;
            
            xx = 0;
            xy = ysign;
            yx = xsign;
            yy = 0;
        }

        int D = 2 * dy - dx;
        int y = 0;

        List<int> outX = new List<int>();
        List<int> outY = new List<int>();

        for (int x = 0; x <= dx; x++)
        {
            int nx = x0 + x * xx + y * yx;
            int ny = y0 + x * xy + y * yy;
            outX.Add(nx);
            outY.Add(ny);
            if (D >= 0)
            {
                y++;
                D -= 2 * dx;
            }
            D += 2 * dy;
        }


        return new Tuple<List<int>, List<int>>(outX, outY);
    }


}
