using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;

[CreateAssetMenu(fileName = "FlowingPath", menuName = "Paths/Flowing Path")]
public class FlowingPath : Path
{
    public float DeltaHeightCost = 1;
    public float LengthCost = 1;
    public float AngleCost = 1;

    public AnimationCurve AngleCostCurve = AnimationCurve.Linear(0,0,1,1);

    public override List<Grid.Position> CalcualtePath(List<Grid.Position> waypoints)
    {
        List<Grid.Position> path = new List<Grid.Position>();
        var pf = new AStarPathFinding<Grid.Position>(_Grid.GetNaighbours, Cost, Heuristic);


        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            var pathSection = pf.Path(waypoints[i], waypoints[i + 1]);

            if (path.Count == 0) path.Add(pathSection.First());

            for (int j = 1; j < pathSection.Count; j++)
            {
                path.Add(pathSection[j]);
            }
        }
        //return RemoveInlinePoints(path, 1f).ToList();
        return path;
    }

    private float Cost(Grid.Position from, Grid.Position to, List<Grid.Position> last)
    {
        var fromPos = _Grid.GetPoint(from);
        var toPos = _Grid.GetPoint(to);
        var dist = Vector3.Distance(fromPos, toPos);

        var heightDelta = (Mathf.Abs(fromPos.y - toPos.y));

        var fromPos2 = new Vector2(from.x, from.y);
        var toPos2 = new Vector2(to.x, to.y);
        var lastPos2 = new Vector2(last[0].x, last[0].y); //AveragePos(last);
        var angle = Mathf.Abs(Vector2.SignedAngle(lastPos2 - fromPos2, toPos2 - fromPos2) / 180);


        return dist * LengthCost + Mathf.Pow(heightDelta, DeltaHeightCost) + AngleCostCurve.Evaluate(angle);
    }

    private Vector2 AveragePos(List<Grid.Position> positions)
    {
        Vector2 pos = new Vector2(positions[0].x, positions[0].y);

        for (int i = 1; i < positions.Count; i++)
        {
            pos += new Vector2(positions[i].x, positions[i].y);
        }

        return pos  / positions.Count;
    }

    private float Heuristic(Grid.Position from, Grid.Position to)
    {
        return Vector2.Distance(new Vector2(from.x, from.y), new Vector2(to.x, to.y));
    }

    public override string GetName()
    {
        return "Flowing Path";
    }

    public override void DrawInspector()
    {
        LengthCost = EditorGUILayout.DelayedFloatField("Length Cost", LengthCost);
        DeltaHeightCost = EditorGUILayout.DelayedFloatField("Height Cost", DeltaHeightCost);
        AngleCost = EditorGUILayout.DelayedFloatField("Angle Cost", AngleCost);
        AngleCostCurve = EditorGUILayout.CurveField("Angle Cost", AngleCostCurve);
    }
}
