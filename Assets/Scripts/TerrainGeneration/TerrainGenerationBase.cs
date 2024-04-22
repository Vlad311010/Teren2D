using Structs;
using UnityEngine;

public abstract class TerrainGenerationBase : MonoBehaviour
{
    public abstract float[,] NoiseGeneration();
    public abstract void GridGeneration();
    public abstract void LayoutGeneration();
    public abstract void AdditionalLayoutGeneration();
    public abstract void PathsGeneration();


    public void Generate(NoiseParameters noiseParam)
    {
        float[,] noise = NoiseGeneration();
        GridGeneration();
        LayoutGeneration();
        AdditionalLayoutGeneration();
        PathsGeneration();
    }

}
