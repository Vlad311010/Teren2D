using Structs;
using System;
using UnityEngine;

[Serializable]
public class TerrainCell
{
    public Vector2Int Coordinates { get => new Vector2Int(x, y); }
    public float NoiseValue => noiseValue;

    private GridSystem<TerrainCell> grid;
    private int x;
    private int y;
    private float noiseValue;

    public TerrainCell(GridSystem<TerrainCell> grid, int x, int y, float noiseValue)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.noiseValue = noiseValue;
    }

    public override string ToString()
    {
        return noiseValue.ToString("P0");
        // return x + "." + y + " : " + noiseValue;
    }
        
}
