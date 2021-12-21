using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathCreator))]
public class BezierPathEditor : Editor
{
    PathCreator _Creator;
    BezierPath _Path;

    void OnEnable()
    {
        _Creator = (PathCreator)target;
        if (_Creator._Path == null) _Creator.CreatePath();
        _Path = _Creator._Path;
    }

    void OnSceneGUI()
    {
        Input();
        Draw();
    }

    void Draw()
    {

        Handles.color = Color.black;
        for (int i = 0; i < _Path.NumPoints-1; i++)
        {
            Handles.DrawLine(_Path[i], _Path[i+1], 5f);
        }

        Handles.color = Color.red;
        for (int i = 0; i < _Path.NumPoints; i++)
        {
            Vector3 newPos = Handles.FreeMoveHandle(_Path[i], Quaternion.identity, HandleUtility.GetHandleSize(_Path[i]), Vector2.zero, Handles.SphereHandleCap);
            if (newPos != _Path[i])
            {
                Undo.RecordObject(_Creator, "Move Point");
                _Path.MovePoint(i, newPos);
            }
        }
    }

    void Input()
    {
        Event guiEvent = Event.current;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        HandleAddPoint(guiEvent, mousePos);
        HandleRemovePoint(guiEvent, mousePos);
    }

    private void HandleRemovePoint(Event guiEvent, Vector2 mousePos)
    {
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && guiEvent.alt)
        {
            float minDstToAncher = 0.05f;
            int closestAncherIndex = -1;

            var cameraRotation = Quaternion.Inverse(SceneView.lastActiveSceneView.rotation);

            for (int i = 0; i < _Path.NumPoints; i++)
            {
                float dst = Vector2.Distance(cameraRotation * mousePos, cameraRotation * _Path[i]);
                if (dst < minDstToAncher)
                {
                    minDstToAncher = dst;
                    closestAncherIndex = i;
                }
            }

            if (closestAncherIndex != -1)
            {
                Undo.RecordObject(_Creator, "Remove Point");
                _Path.RemovePoint(closestAncherIndex);
            }
        }
    }

    private void HandleAddPoint(Event guiEvent, Vector2 mousePos)
    {
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.alt)
        {
            Undo.RecordObject(_Creator, "Add Point");
            _Path.AddPoint(mousePos);
        }
    }
}
