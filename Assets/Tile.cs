using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Normal,
        Hazard,
        Target,
        Gold
    }

    public TileType tileType = TileType.Normal;

    private Renderer tileRenderer;

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        UpdateColor();
    }

    public void SetTileType(TileType newType)
    {
        tileType = newType;
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (tileRenderer == null)
        {
            tileRenderer = GetComponent<Renderer>();
        }

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
        else if (tileType == TileType.Gold)
        {
            tileRenderer.material.color =
                new Color(1f, 0.65f, 0f);
        }
    }
}