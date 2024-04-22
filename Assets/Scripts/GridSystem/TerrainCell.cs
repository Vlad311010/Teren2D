using System;

[Serializable]
public class TerrainCell : GridCellBase
{
    public float NoiseValue { get => noiseValue; set => noiseValue = value; }

    private GridSystem<TerrainCell> grid;
    private float noiseValue;

    public TerrainCell(GridSystem<TerrainCell> grid, int x, int y, float noiseValue) : base(x, y)
    {
        this.grid = grid;
        this.noiseValue = noiseValue;
    }

    public override string ToString()
    {
        return x + "." + y + " : " + noiseValue.ToString("P0");
    }

}
