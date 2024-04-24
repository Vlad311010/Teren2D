
using System;

namespace Enums
{
    public enum TileType
    {
        Ground = 0,
        GrassGround = 1,
        Grass = 2,
        Tree = 3
    }

    public enum AgentRotationDirection
    {
        None = 0,
        Left = 1,
        Right = 2,
        TurnAround = 3
    }

    [Flags]
    public enum WorldSides
    {
        None = 0,
        North = 1 << 0,
        East = 1 << 1,
        South = 1 << 2,
        West = 1 << 3,
    }
}