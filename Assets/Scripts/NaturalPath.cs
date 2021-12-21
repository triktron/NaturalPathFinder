using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Terrain), typeof(TerrainCollider))]
public class NaturalPath : MonoBehaviour
{
    private List<Vector2> _Points = new List<Vector2>();
    private Terrain _Terrain;
    private TerrainCollider _TerrainCollider;

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
        }
    }

    public void AddPoint(Vector3 pos)
    {
        Vector3 localPoint = pos - transform.position;
        _Points.Add(new Vector2(localPoint.x, localPoint.z));
    }

    public void InsertPoint(int index, Vector3 pos)
    {
        Vector3 localPoint = pos - transform.position;
        _Points.Insert(index+1, new Vector2(localPoint.x, localPoint.z));
    }

    public void RemovePoint(int index)
    {
        _Points.RemoveAt(index);
    }
}
