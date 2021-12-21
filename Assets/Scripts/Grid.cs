using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grid
{
    public struct Position
    {
        public Position(int i, Vector2Int segments)
        {
            this.i = i;
            this.x = Mathf.FloorToInt(i / segments.y);
            this.y = i % segments.y;
        }

        public Position(int x, int y, Vector2Int segments)
        {
            this.i = x * segments.y + y;
            this.x = x;
            this.y = y;
        }

        public int x;
        public int y;
        public int i;
    }

    private Vector2Int _Segments;
    private Vector2 _Size;
    private Vector3[] _Grid;
    private Terrain _Terrain;

    public Grid(Terrain terrain, float gridSize)
    {
        _Terrain = terrain;
        SetGridSize(gridSize);
    }

    public void SetGridSize(float gridSize)
    {
        var terrianSize = _Terrain.terrainData.size;

        int xSegments = Mathf.RoundToInt(terrianSize.x / gridSize);
        int ySegments = Mathf.RoundToInt(terrianSize.z / gridSize);
        var xSize = terrianSize.x / xSegments;
        var ySize = terrianSize.z / ySegments;

        _Size = new Vector2(xSize, ySize);
        _Segments = new Vector2Int(xSegments+1, ySegments+1);

        InitializeGrid();
    }

    public void InitializeGrid()
    {
        _Grid = new Vector3[_Segments.x * _Segments.y];
        for (int x = 0; x < _Segments.x; x++)
        {
            for (int y = 0; y < _Segments.y; y++)
            {
                var pos = new Vector3(x * _Size.x, 0, y * _Size.y) + _Terrain.transform.position;
                pos.y = _Terrain.SampleHeight(pos) + _Terrain.transform.position.y;
                _Grid[x * _Segments.y + y] = pos;
            }
        }
    }

    public void DrawGridHandles(float handleSize = 2f)
    {
        for (int x = 0; x < _Segments.x; x++)
        {
            for (int y = 0; y < _Segments.y; y++)
            {
                Handles.DrawSolidDisc(_Grid[x * _Segments.y + y], Vector3.up, handleSize);
            }
        }
    }

    public Vector3 SnapToWorld(Vector3 pos)
    {
        var local = pos - _Terrain.transform.position;
        int x = Mathf.RoundToInt(local.x / _Size.x);
        int y = Mathf.RoundToInt(local.z / _Size.y);

        return _Grid[x * _Segments.y + y];
    }

    public Position SnapToPosition(Vector3 pos)
    {
        var local = pos - _Terrain.transform.position;
        int x = Mathf.RoundToInt(local.x / _Size.x);
        int y = Mathf.RoundToInt(local.z / _Size.y);

        return new Position(x, y, _Segments);
    }

    public Position SnapToPosition(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x / _Size.x);
        int y = Mathf.RoundToInt(pos.y / _Size.y);

        return new Position(x, y, _Segments);
    }


    public Vector2 GetSize() => _Size;
    public Vector2Int GetSegments() => _Segments;
    public Vector3 GetPoint(Position pos) => _Grid[pos.i];
}
