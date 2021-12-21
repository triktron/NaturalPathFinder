using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    
    [HideInInspector] public BezierPath _Path;

    public void CreatePath()
    {
        _Path = new BezierPath();
    }
}
