using kmty.NURBS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[CreateAssetMenu(fileName = "ShortestPath", menuName = "Paths/Shortest Path")]
public class ShortestPath : Path
{
    public override List<Grid.Position> CalcualtePath(List<Grid.Position> waypoints)
    {
        List<Grid.Position> path = new List<Grid.Position>();
        var pf = new AStarPathFinding<Grid.Position>(Neighbours, Distance, Distance);


        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            var pathSection = pf.Path(waypoints[i], waypoints[i + 1]);

            if (path.Count == 0) path.Add(pathSection.First());

            for (int j = 1; j < pathSection.Count; j++)
            {
                path.Add(pathSection[j]);
            }
        }
        return RemoveInlinePoints(path, 1f).ToList();
    }

    private List<Grid.Position> Neighbours(Grid.Position from)
    {
        var neighbours = new List<Grid.Position>();
        var segments = _Grid.GetSegments();

        if (from.x > 0) neighbours.Add(new Grid.Position(from.x - 1, from.y, segments));
        if (from.x < segments.x - 1) neighbours.Add(new Grid.Position(from.x + 1, from.y, segments));
        if (from.y > 0) neighbours.Add(new Grid.Position(from.x, from.y - 1, segments));
        if (from.y < segments.y - 1) neighbours.Add(new Grid.Position(from.x, from.y + 1, segments));
        if (from.x > 0 && from.y > 0) neighbours.Add(new Grid.Position(from.x - 1, from.y - 1, segments));
        if (from.x < segments.x - 1 && from.y > 0) neighbours.Add(new Grid.Position(from.x + 1, from.y - 1, segments));
        if (from.x > 0 && from.y < segments.y - 1) neighbours.Add(new Grid.Position(from.x - 1, from.y + 1, segments));
        if (from.x < segments.x - 1 && from.y < segments.y - 1) neighbours.Add(new Grid.Position(from.x + 1, from.y + 1, segments));


        return neighbours;
    }

    private float Distance(Grid.Position from, Grid.Position to)
    {
        return Vector2.Distance(new Vector2(from.x, from.y), new Vector2(to.x, to.y));
    }

    public override string GetName()
    {
        return "Shortest Path";
    }
}
