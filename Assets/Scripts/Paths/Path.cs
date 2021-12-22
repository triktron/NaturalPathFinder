using kmty.NURBS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class Path: ScriptableObject
{
    public bool DrawGridLines = false;
    public bool DrawSplineLines = false;
    public Color GridLineColor = Color.cyan;
    public Color SplineColor = Color.red;

    public Grid _Grid;

    private List<Grid.Position> _Waypoints;

    private Cache<Vector3> _PathNodes;
    private Cache<Vector3> _SplineNodes;
    public Path()
    {
        _PathNodes = new Cache<Vector3>(() =>
        {
            return CalcualtePath(_Waypoints).Select(n => _Grid.GetPoint(n)).ToArray();
        });

        _SplineNodes = new Cache<Vector3>(() =>
        {
            var nodes = _PathNodes.GetValue();
            var spline = new Spline(nodes.Select(p => new CP(p, 1)).ToArray(), 4);

            var segCount = 2 * nodes.Length;
            var seg = 1f / segCount;
            var splinePoints = new Vector3[segCount+1];

            for (int i = 0; i < segCount; i++)
            {
                var t = seg * i;
                splinePoints[i] = spline.GetCurve(t);
            }
            splinePoints[segCount] = nodes.Last();

            return splinePoints;
        });
    }

    public virtual void Init(Grid grid)
    {
        _Grid = grid;
    }

    public abstract List<Grid.Position> CalcualtePath(List<Grid.Position> waypoints);
    public abstract string GetName();

    public Vector3[] GetPathNodes()
    {
        return _PathNodes.GetValue();
    }

    public void SetWaypoints(List<Grid.Position> waypoints)
    {
        _Waypoints = waypoints;
        _PathNodes.SetDirty();
        _SplineNodes.SetDirty();
    }

    public void DrawHandlesGrid()
    {
        if (DrawGridLines)
        {
            var path = _PathNodes.GetValue();

            Handles.color = GridLineColor;
            for (int i = 0; i < path.Length - 1; i++)
            {
                Handles.DrawSolidDisc(path[i], Vector3.up, 2);
                Handles.DrawLine(path[i], path[i + 1], 1);
            }

            if (path.Length != 0) Handles.DrawSolidDisc(path.Last(), Vector3.up, 2);
        }
    }

    public void DrawHandlesSpline()
    {
        if (DrawSplineLines)
        {
            var spline = _SplineNodes.GetValue();

            Handles.color = SplineColor;
            Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 4, spline);
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
