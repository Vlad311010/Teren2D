using UnityEngine;

public abstract class TerrainGenerationBase : MonoBehaviour
{
    protected abstract void Clean();
    protected abstract void Init(int seed);
    protected abstract void NoiseGeneration();
    protected abstract void LayoutGeneration();
    protected abstract void LayoutPostprocessing();
    protected abstract void FillTilemaps();
    protected abstract void PathsGeneration();
    protected abstract void PropsPlacing();


    public void Generate(int seed)
    {
        Clean();
        Init(seed);
        NoiseGeneration();
        LayoutGeneration();
        LayoutPostprocessing();
        FillTilemaps();
        PathsGeneration();
        PropsPlacing();
    }

}
