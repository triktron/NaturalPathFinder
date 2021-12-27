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
        var pf = new AStarPathFinding<Grid.Position>(_Grid.GetNaighbours, Distance, Heuristic);


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

    private float Distance(Grid.Position from, Grid.Position to, List<Grid.Position> last)
    {
        return Vector3.Distance(_Grid.GetPoint(from), _Grid.GetPoint(to));
    }
    private float Heuristic(Grid.Position from, Grid.Position to)
    {
        return Vector3.Distance(_Grid.GetPoint(from), _Grid.GetPoint(to));
    }

    public override string GetName()
    {
        return "Shortest Path";
    }
}
