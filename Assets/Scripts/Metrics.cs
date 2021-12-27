using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metrics
{
    public struct Metric
    {
        public float Length;
        public float HeightDelta;
        public float AvgSlope;
        public float Curvature;
        public float AvgCurvature;
    }

    private Cache<Vector3[]> _Path;
    private Cache<Metric> _Metric;

    public Metrics()
    {
        _Metric = new Cache<Metric>(UpdateMetric);
    }

    public void SetDirty()
    {
        _Metric.SetDirty();
    }

    public void SetPath(Cache<Vector3[]> path)
    {
        _Path = path;
    }

    public Metric GetMetric() => _Metric.GetValue();

    private Metric UpdateMetric()
    {
        var path = _Path.GetValue();
        var metric = new Metric();

        var previousPoint = path[0];
        for (int i = 1; i < path.Length; i++)
        {
            var currentPoint = path[i];
            var nextPoint = (i+1 < path.Length) ? path[i+1] : path[i];
            var previousPoint2d = new Vector2(previousPoint.x, previousPoint.z);
            var currentPoint2d = new Vector2(currentPoint.x, currentPoint.z);
            var nextPoint2d = new Vector2(nextPoint.x, nextPoint.z);
            var heightDiff = previousPoint.y - currentPoint.y;


            metric.Length += Vector3.Distance(previousPoint, currentPoint);
            metric.HeightDelta += Mathf.Abs(heightDiff);

            var flatDist = Vector2.Distance(previousPoint2d, currentPoint2d);
            var slope = Mathf.Abs(Mathf.Atan2(heightDiff, flatDist)) * Mathf.Rad2Deg;
            metric.AvgSlope += slope;

            if (i + 1 < path.Length) metric.Curvature += 180 - Mathf.Abs(Vector2.SignedAngle(previousPoint2d - currentPoint2d, nextPoint2d - currentPoint2d));

            previousPoint = currentPoint;
        }

        metric.AvgSlope /= path.Length;
        metric.AvgCurvature = metric.Curvature / path.Length;

        return metric;
    }
}
