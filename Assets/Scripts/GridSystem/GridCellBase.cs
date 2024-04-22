using UnityEngine;

public class GridCellBase
{
    public Vector2Int Coordinates { get => new Vector2Int(x, y); }
    
    protected int x;
    protected int y;

    public GridCellBase(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return x + "." + y;
    }
}
