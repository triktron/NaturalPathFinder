using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierPath
{
    [SerializeField, HideInInspector]
    List<Vector3> _Points;

    public BezierPath()
    {
        _Points = new List<Vector3>()
        {
            Vector3.zero
        };
    }

    public int NumPoints
    {
        get
        {
            return _Points.Count;
        }
    }

    public Vector3 this[int i]
    {
        get
        {
            return _Points[i];
        }
    }

    public void MovePoint(int i, Vector3 pos)
    {
        _Points[i] = pos;
    }

    public void AddPoint(Vector3 pos)
    {
        _Points.Add(pos);
    }

    public void RemovePoint(int i)
    {
        _Points.RemoveAt(i);
    }
}
