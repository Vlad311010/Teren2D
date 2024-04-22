using UnityEngine;

public class PerlinNoise
{
    const int randomRange = 10000;

    public static float[,] GetNoiseMap(int width, int height, Vector2 offset, float scale, int octaves, float persistance, float lacunarity, int seed)
    {
        float[,] noiseMap = new float[width, height];

        scale = scale == 0 ? 0.0001f : scale;
        Vector2[] octavesOffsets;
        System.Random random;
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        float halfWidth = width / 2;
        float halfHeight = height / 2;

        random = new System.Random(seed);
        octavesOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octavesOffsets[i] = new Vector2(
                random.Next(-randomRange, randomRange) + offset.x, 
                random.Next(-randomRange, randomRange) + offset.y
                );
        }

        // calculate noise height
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale + octavesOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale + octavesOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxHeight)
                    maxHeight = noiseHeight;  
                else if (noiseHeight < minHeight)
                    minHeight = noiseHeight;

                noiseMap[x,y] = noiseHeight;

            }  
        }

        // normalize height values
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

}
