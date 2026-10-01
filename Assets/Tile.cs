using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Normal,
        Hazard,
        Target,
        Warning,
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

        if (tileRenderer == null)
            return;

        // Turn emission off by default
        tileRenderer.material.DisableKeyword("_EMISSION");

        switch (tileType)
        {
            case TileType.Normal:

                tileRenderer.material.color =
                    Color.blue;

                break;

            case TileType.Hazard:

                tileRenderer.material.color =
                    Color.red;

                break;

            case TileType.Target:

                tileRenderer.material.color =
                    Color.green;

                break;

            case TileType.Warning:

                tileRenderer.material.color =
                    Color.orange;

                break;

            case TileType.Gold:

                // Bright yellow
                Color goldColor =
                    new Color(
                        1f,
                        0.9f,
                        0.05f
                    );

                tileRenderer.material.color =
                    goldColor;

                // Emissive yellow
                tileRenderer.material.EnableKeyword(
                    "_EMISSION"
                );

                tileRenderer.material.SetColor(
                    "_EmissionColor",
                    goldColor * 4f
                );

                break;
        }
    }
}