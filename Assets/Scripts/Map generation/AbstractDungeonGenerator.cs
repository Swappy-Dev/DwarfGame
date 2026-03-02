using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TileMapVisualization tileMapVisualization = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void GenerateDungeon()
    {
        tileMapVisualization.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
