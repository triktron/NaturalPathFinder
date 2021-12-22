using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Terrain), typeof(TerrainCollider))]
public class NaturalPath : MonoBehaviour
{
    [HideInInspector] public bool DrawGrid = false;
    [HideInInspector] public bool DrawWaypoints = false;

    [SerializeField]
    private List<Vector2> _Points = new List<Vector2>();
    [SerializeField, HideInInspector]
    private float _GridSize = 50f;

    public float GridSize
    {
        get => _GridSize;
        set
        {
            _GridSize = value;
            GetGrid().SetGridSize(_GridSize);
            UpdatePaths();
        }
    }


    private Terrain _Terrain;
    private TerrainCollider _TerrainCollider;
    private Grid _Grid;

    private bool _PathsInitialized = false;
    [SerializeField]
    private Path[] _Paths = new Path[0]
    {
    };

    public Terrain GetTerrain()
    {
        if (_Terrain == null) _Terrain = GetComponent<Terrain>();
        return _Terrain;
    }

    public TerrainCollider GetTerrainCollider()
    {
        if (_TerrainCollider == null) _TerrainCollider = GetComponent<TerrainCollider>();
        return _TerrainCollider;
    }

    public Grid GetGrid()
    {
        if (_Grid == null) _Grid = new Grid(GetTerrain(), _GridSize);
        return _Grid;
    }

    public Path[] GetPaths()
    {
        //if (!_PathsInitialized)
        //{
            //_PathsInitialized = true;

            foreach (var path in _Paths)
            {
                path.Init(GetGrid());
            }
        //}
        return _Paths;
    }

    public int PointCount => _Points.Count;
    public List<Vector2> LocalPoints => _Points;

    public Vector3[] WorldPoints => _Points.Select(p => SampleWorldPosition(p)).ToArray();

    public Vector3 GetWorldPoint(int index)
    {
        if (index < 0 || index >= _Points.Count) return Vector3.zero;

        Vector2 localPoint = _Points[index];

        return SampleWorldPosition(localPoint);
    }

    public Vector3 SampleWorldPosition(Vector2 local)
    {
        var worldPos = new Vector3(local.x, 0, local.y) + transform.position;

        var height = GetTerrain().SampleHeight(worldPos);

        worldPos.y = height + transform.position.y;

        return worldPos;
    }

    public void SetWorldPoint(int index, Vector3 pos)
    {
        if (index >= 0 && index < _Points.Count)
        {
            Vector3 localPoint = pos - transform.position;

            _Points[index] = new Vector2(localPoint.x, localPoint.z);
            UpdatePaths();
        }
    }

    public void UpdatePaths()
    {
        var waypoints = _Points.Select(p => GetGrid().SnapToPosition(p)).ToList();

        foreach (var path in GetPaths())
        {
            path.SetWaypoints(waypoints);
        }
    }

    public void AddPoint(Vector3 pos)
    {
        Vector3 localPoint = pos - transform.position;
        _Points.Add(new Vector2(localPoint.x, localPoint.z));
        UpdatePaths();
    }

    public void InsertPoint(int index, Vector3 pos)
    {
        Vector3 localPoint = pos - transform.position;
        _Points.Insert(index+1, new Vector2(localPoint.x, localPoint.z));
        UpdatePaths();
    }

    public void RemovePoint(int index)
    {
        _Points.RemoveAt(index);
        UpdatePaths();
    }
}
