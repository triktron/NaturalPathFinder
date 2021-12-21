using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class Path
{
    public Grid _Grid;
    public Spline _Spline;

    public Grid.Position[] PathNodes = new Grid.Position[0];

    public virtual void Init(Grid grid)
    {
        _Grid = grid;
        _Spline = new Spline();
    }

    public abstract void CalcualtePath(List<Grid.Position> waypoints);

    public void DrawHandles()
    {
        Handles.color = Color.cyan;
        for (int i = 0; i < PathNodes.Length - 1; i++)
        {
            Handles.DrawSolidDisc(_Grid.GetPoint(PathNodes[i]), Vector3.up, 2);
            Handles.DrawLine(_Grid.GetPoint(PathNodes[i]), _Grid.GetPoint(PathNodes[i + 1]), 1);
        }

        if (PathNodes.Length != 0) Handles.DrawSolidDisc(_Grid.GetPoint(PathNodes.Last()), Vector3.up, 2);

        if (PathNodes.Length >= 4)
        {
            _Spline.Points = PathNodes.Select(p => _Grid.GetPoint(p)).ToArray();
            _Spline.DrawHandles();
        }
    }

    public List<Grid.Position> RemoveInlinePoints(List<Grid.Position> points, float maxDist) {
        var remove = new List<int>();

        for (int i = 1; i < points.Count - 1; i++)
        {
            var dist = HandleUtility.DistancePointLine(_Grid.GetPoint(points[i]), _Grid.GetPoint(points[i-1]), _Grid.GetPoint(points[i+1]));
            if (dist < maxDist) remove.Add(i);
        }

        var newPoints = new List<Grid.Position>();

        for (int i = 0; i < points.Count; i++)
        {
            if (!remove.Contains(i)) newPoints.Add(points[i]);
        }

        return newPoints;
    }
}
