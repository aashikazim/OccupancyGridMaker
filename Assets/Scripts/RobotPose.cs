using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPose
{
    public float x;
    public float y;
    public float theta;
    public List<float> range;

    public override string ToString()
    {
        string rangeString = range != null ? "[" + string.Join(", ", range) + "]" : "null";
        return $"RobotPose: x = {x}, y = {y}, theta = {theta}, range = {rangeString}";
    }
}
