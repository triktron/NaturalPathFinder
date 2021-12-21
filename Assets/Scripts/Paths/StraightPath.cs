using kmty.NURBS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StraightPath : Path
{
    public override void CalcualtePath(List<Grid.Position> waypoints)
    {
        List<Grid.Position> path = new List<Grid.Position>();


        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            var pathSection = Line(waypoints[i], waypoints[i + 1]);

            if (path.Count == 0) path.Add(pathSection.First());

            for (int j = 1; j < pathSection.Count; j++)
            {
                path.Add(pathSection[j]);
            }
        }

        PathNodes = RemoveInlinePoints(path,1f).ToArray();

        _Spline = new Spline(PathNodes.Select(p => new CP(_Grid.GetPoint(p),1)).ToArray(), 4);
    }

    public List<Grid.Position> Line(Grid.Position from, Grid.Position to)
    {
        List<Grid.Position> path = new List<Grid.Position>();
        int x = from.x, y = from.y, x2 = to.x, y2 = to.y;
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            path.Add(new Grid.Position(x,y,_Grid.GetSegments()));

            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }

        return path;
    }

    public override string GetName()
    {
        return "Straight Path";
    }
}
