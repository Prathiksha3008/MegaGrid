using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Normal,
        Hazard,
        Target
    }

    public TileType tileType = TileType.Normal;

    private Renderer tileRenderer;

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        UpdateColor();
    }

    void UpdateColor()
    {
        if (tileType == TileType.Normal)
        {
            tileRenderer.material.color = Color.blue;
        }
        else if (tileType == TileType.Hazard)
        {
            tileRenderer.material.color = Color.red;
        }
        else if (tileType == TileType.Target)
        {
            tileRenderer.material.color = Color.green;
        }
    }
}