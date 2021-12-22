using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NaturalPath))]
public class NaturalPathEditor : Editor
{
    private NaturalPath _Path;
    private static float _HandleSize = 4;

    void OnEnable()
    {
        _Path = (NaturalPath)target;
        _Path.UpdatePaths();
    }

    void OnSceneGUI()
    {
        Draw();
        Input();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Waypoint Settings", EditorStyles.boldLabel);
        _Path.DrawWaypoints = EditorGUILayout.Toggle("Draw Waypoints", _Path.DrawWaypoints);
        _HandleSize = EditorGUILayout.FloatField("Handle Size", _HandleSize);


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _Path.DrawGrid = EditorGUILayout.Toggle("Draw DrawGrid", _Path.DrawGrid);
        var newGridSize = EditorGUILayout.DelayedFloatField("Grid Size", _Path.GridSize);
        if (newGridSize != _Path.GridSize)
        {
            _Path.GridSize = newGridSize;
        }
        EditorGUILayout.EndHorizontal();

        foreach (var path in _Path.GetPaths())
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(path.GetName() + " Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            path.DrawGridLines = EditorGUILayout.Toggle("Draw Grid Lines", path.DrawGridLines);
            path.DrawSplineLines = EditorGUILayout.Toggle("Draw Spline Lines", path.DrawSplineLines);
            path.GridLineColor = EditorGUILayout.ColorField("Grid Line Color", path.GridLineColor);
            path.SplineColor = EditorGUILayout.ColorField("Spline Color", path.SplineColor);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(path);
            }
        }
    }

    void Draw()
    {
        if (_Path.DrawGrid) _Path.GetGrid().DrawGridHandles(_HandleSize);

        foreach (var path in _Path.GetPaths())
        {
            path.DrawHandlesGrid();

            path.DrawHandlesSpline();
        }

        if (_Path.DrawWaypoints) {
            Handles.color = Color.green;
            for (int i = 0; i < _Path.PointCount - 1; i++)
            {
                Handles.DrawLine(_Path.GetWorldPoint(i), _Path.GetWorldPoint(i + 1), 1);
            }

            Handles.color = Color.red;
            for (int i = 0; i < _Path.PointCount; i++)
            {
                Vector3 pos = _Path.GetWorldPoint(i);
                Vector3 newPos = Handles.FreeMoveHandle(pos, Quaternion.identity, _HandleSize, Vector2.zero, Handles.SphereHandleCap);
                if (newPos != pos)
                {
                    Undo.RecordObject(_Path, "Move Point");
                    if (RaycastTerrain(out var hit))
                        _Path.SetWorldPoint(i, hit);
                }
            }
        }
    }

    void Input()
    {
        Event guiEvent = Event.current;
        if (guiEvent.type == EventType.MouseDown && guiEvent.alt)
        {
            if (guiEvent.button == 0)
            {
                if (!HandleInsertPoint()) HandleAddPoint();
            }
            if (guiEvent.button == 1) HandleRemovePoint();
        }
    }

    private bool HandleInsertPoint()
    {
        Event guiEvent = Event.current;
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.alt)
        {
            var cursor = (Event.current.mousePosition);
            var closestDist = 10f;
            var closestIndex = -1;

            for (int i = 0; i < _Path.PointCount - 1; i++)
            {
                var pointA = HandleUtility.WorldToGUIPoint(_Path.GetWorldPoint(i));
                var pointB = HandleUtility.WorldToGUIPoint(_Path.GetWorldPoint(i + 1));

                var dist = HandleUtility.DistancePointLine(cursor, pointA, pointB);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }



            if (closestIndex > -1)
            {
                if (RaycastTerrain(out var hit))
                {
                    Undo.RecordObject(_Path, "Insert Point");
                    _Path.InsertPoint(closestIndex, hit);
                    Event.current.Use();
                    return true;
                }
            }
        }

        return false;
    }

    private void HandleAddPoint()
    {
        if (RaycastTerrain(out var hit))
        {
            Undo.RecordObject(_Path, "Add Point");
            _Path.AddPoint(hit);
            Event.current.Use();
        }
    }

    private void HandleRemovePoint()
    {
        float minDstToAncher = _HandleSize / 2;
        int closestAncherIndex = -1;

        if (RaycastTerrain(out var hit))
        {
            for (int i = 0; i < _Path.PointCount; i++)
            {
                float dst = Vector2.Distance(hit, _Path.GetWorldPoint(i));
                if (dst < minDstToAncher)
                {
                    minDstToAncher = dst;
                    closestAncherIndex = i;
                }
            }

            if (closestAncherIndex != -1)
            {
                Undo.RecordObject(_Path, "Remove Point");
                _Path.RemovePoint(closestAncherIndex);
                Event.current.Use();
            }
        }
    }

    bool RaycastTerrain(out Vector3 hitPosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (_Path.GetTerrainCollider().Raycast(ray, out var hit, 100000))
        {
            hitPosition = hit.point;
            return true;
        }
        hitPosition = Vector3.zero;
        return false;
    }
}
