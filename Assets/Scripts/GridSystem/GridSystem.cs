using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GridSystem<T> where T : GridCellBase
{
    public int Width => width;
    public int Height => height;

    public event EventHandler<OnCellChangedEventArgs> OnCellChanged;
    public class OnCellChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    int width;
    int height;
    float cellSize;
    Vector3 origin;
    T[,] grid;

    public GridSystem(int width, int height, float cellSize, Vector3 origin, Func<GridSystem<T>, int, int, T> createCell)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;
        grid = new T[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = createCell(this, x, y);
            }
        }
    }

    public Vector3 GetWorldPosition(Vector2Int coordinates)
    {
        return GetWorldPosition(coordinates.x, coordinates.y);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + origin;
    }

    public Vector2Int GetXY(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
        int y = Mathf.FloorToInt((worldPosition - origin).z / cellSize);
        return new Vector2Int(x, y);
    }

    public bool ValidateCoordinates(Vector2Int coordinates)
    {
        return ValidateCoordinates(coordinates.x, coordinates.y);
    }

    public bool ValidateCoordinates(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public Vector2Int RandomPoint()
    {
        int x = UnityEngine.Random.Range(0, width);
        int y = UnityEngine.Random.Range(0, height);
        return new Vector2Int(x, y);
    }


    public T GetCell(Vector2Int pos)
    {
        return GetCell(pos.x, pos.y);
    }

    public T GetCell(int x, int y)
    {
        return grid[x, y];
    }

    public List<T> GetNeighboursCardinal(Vector2Int coordinates)
    {
        return GetNeighboursCardinal(coordinates.x, coordinates.y);
    }

    public List<T> GetNeighboursCardinal(int x, int y)
    {
        List<T> neighbours = new List<T>();

        Vector2Int left = new Vector2Int(x - 1, y);
        Vector2Int right = new Vector2Int(x + 1, y);
        Vector2Int up = new Vector2Int(x, y + 1);
        Vector2Int down = new Vector2Int(x, y - 1);

        if (ValidateCoordinates(left))
            neighbours.Add(GetCell(left));

        if (ValidateCoordinates(right))
            neighbours.Add(GetCell(right));

        if (ValidateCoordinates(up))
            neighbours.Add(GetCell(up));

        if (ValidateCoordinates(down))
            neighbours.Add(GetCell(down));

        return neighbours;
    }

    public List<T> GetNeighboursDiagonal(int x, int y)
    {
        List<T> neighbours = new List<T>();

        Vector2Int upLeft = new Vector2Int(x - 1, y + 1);
        Vector2Int upRight = new Vector2Int(x + 1, y + 1);
        Vector2Int donwLeft = new Vector2Int(x - 1, y - 1);
        Vector2Int downRight = new Vector2Int(x + 1, y - 1);

        if (ValidateCoordinates(upLeft))
            neighbours.Add(GetCell(upLeft));

        if (ValidateCoordinates(upRight))
            neighbours.Add(GetCell(upRight));

        if (ValidateCoordinates(donwLeft))
            neighbours.Add(GetCell(donwLeft));

        if (ValidateCoordinates(downRight))
            neighbours.Add(GetCell(downRight));

        return neighbours;
    }


    public List<T> GetNeighboursAll(Vector2Int coordinates)
    {
        return GetNeighboursAll(coordinates.x, coordinates.y);
    }

    public List<T> GetNeighboursAll(int x, int y)
    {
        List<T> neighbours = GetNeighboursCardinal(x, y);
        neighbours.AddRange(GetNeighboursDiagonal(x, y));
        return neighbours;
    }


    public void SetCell(int x, int y, T cell)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            grid[x, y] = cell;
            if (OnCellChanged != null)
            {
                OnCellChanged(this, new OnCellChangedEventArgs { x = x, y = y });
            }
        }
    }

    public void TriggerCellhanged(int x, int y)
    {
        if (OnCellChanged != null)
        {
            OnCellChanged(this, new OnCellChangedEventArgs { x = x, y = y });
        }
    }

    public void DebugDraw()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++) 
            {
                Vector3 cellPos = GetWorldPosition(x, y);
                Debug.DrawLine(cellPos, GetWorldPosition(x + 1, y), Color.white);
                Debug.DrawLine(cellPos, GetWorldPosition(x, y + 1), Color.white);

                /*#if UNITY_EDITOR
                    GUI.color = Color.black;
                    Handles.Label(cellPos + Vector3.one * cellSize / 2, grid[x,y].ToString());
                #endif*/
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white);

        
    }
}
