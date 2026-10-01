using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldenTileManager : MonoBehaviour
{
    private Tile[] tiles;

    private Tile goldenTile;

    private RedWaveManager redWaveManager;

    private bool goldSpawned = false;

    void Start()
    {
        tiles =
            FindObjectsOfType<Tile>();

        redWaveManager =
            FindObjectOfType<RedWaveManager>();

        StartCoroutine(
            WaitForLevelComplete()
        );
    }

    IEnumerator WaitForLevelComplete()
    {
        while (redWaveManager != null &&
               !redWaveManager.levelCompleted)
        {
            yield return null;
        }

        if (!goldSpawned)
        {
            SpawnGoldenTile();
        }
    }

    void SpawnGoldenTile()
    {
        // Refresh tile list in case grid was generated
        // after this manager started.
        tiles =
            FindObjectsOfType<Tile>();

        List<Tile> availableTiles =
            new List<Tile>();

        foreach (Tile tile in tiles)
        {
            if (tile.tileType ==
                Tile.TileType.Normal)
            {
                availableTiles.Add(tile);
            }
        }

        if (availableTiles.Count == 0)
        {
            Debug.LogWarning(
                "No available tile for Golden Tile!"
            );

            return;
        }

        int randomIndex =
            Random.Range(
                0,
                availableTiles.Count
            );

        goldenTile =
            availableTiles[randomIndex];

        goldenTile.UpdateColor(
            Tile.TileType.Gold
        );

        goldSpawned = true;

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "GOLD TILE APPEARED!"
        );

        Debug.Log(
            "Position: (" +
            Mathf.RoundToInt(
                goldenTile.transform.position.x
            ) +
            "," +
            Mathf.RoundToInt(
                goldenTile.transform.position.z
            ) +
            ")"
        );

        Debug.Log(
            "=============================="
        );
    }
}