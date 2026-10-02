using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GoldenTileManager : MonoBehaviour
{
    private Tile[] tiles;

    private Tile goldenTile;

    private RedWaveManager redWaveManager;

    private bool goldSpawned = false;

    // In case that the goal tile can't be generated, simply fallback to the win screen instead.
    private PlayerMovement fallbackVictory;

    void Start()
    {
        tiles = FindObjectsByType<Tile>();

        redWaveManager = FindAnyObjectByType<RedWaveManager>();

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
        tiles = FindObjectsByType<Tile>();

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
                "No available tile for Golden Tile! Falling back to player winning instead."
            );

            fallbackVictory.WinGame();

            return;
        }

        int randomIndex =
            Random.Range(
                0,
                availableTiles.Count
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

        goldenTile =
            availableTiles[randomIndex];

        goldenTile.UpdateColor(
            Tile.TileType.Gold
        );
    }
}