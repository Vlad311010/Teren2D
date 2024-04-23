
using Enums;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Structs
{
    public struct NoiseParameters
    {
        public int width;
        public int height;
        public Vector2 offset;
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public int seed;

        public NoiseParameters(int width, int height, Vector2 offset, float scale, int octaves, float persistance, float lacunarity, int seed)
        {
            this.width = width;
            this.height = height;
            this.offset = offset;
            this.scale = scale;
            this.octaves = octaves;
            this.persistance = persistance;
            this.lacunarity = lacunarity;
            this.seed = seed;
        }
    }


    [Serializable]
    public struct NoiseClampData
    {
        public int layer;
        public TileProbability[] tiles;
        public float clampValue;

        public TileBase GetRandomTile(System.Random rng)
        {
            float value = rng.UnitInterval();
            for (int i = 0; i < tiles.Length; i++)
            {
                if (value < tiles[i].highProbability)
                    return tiles[i].tile;
            }
            return tiles[0].tile;
        }
    }

    [Serializable]
    public struct TileProbability
    {
        public TileBase tile;   
        public float highProbability;
    }
}
