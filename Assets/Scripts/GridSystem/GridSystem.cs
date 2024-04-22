using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GridSystem<T>
{
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

    public T GetCell(Vector2Int pos)
    {
        return GetCell(pos.x, pos.y);
    }

    public T GetCell(int x, int y)
    {
        return grid[x, y];
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

                #if UNITY_EDITOR
                    GUI.color = Color.black;
                    Handles.Label(cellPos + Vector3.one * cellSize / 2, grid[x,y].ToString());
                #endif
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white);

        
    }
}
