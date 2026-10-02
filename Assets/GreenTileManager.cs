using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GreenTileManager : MonoBehaviour
{
    [Header("Green Tile Settings")]

    public int maxGreenTiles = 3;

    public float greenLifetime = 5f;

    public float spawnInterval = 1.0f;

    private Tile[] tiles;

    private List<Tile> activeGreenTiles =
        new List<Tile>();

    private RedWaveManager redWaveManager;

    void Start()
    {
        redWaveManager = FindAnyObjectByType<RedWaveManager>();

        StartCoroutine(
            GreenTileRoutine()
        );
    }

    IEnumerator GreenTileRoutine()
    {
        // Give the grid time to initialize
        yield return null;

        RefreshTiles();

        while (true)
        {
            if (redWaveManager != null &&
                redWaveManager.GameStopped)
            {
                yield break;
            }

            //RemoveInvalidTiles();

            while (
                activeGreenTiles.Count <
                maxGreenTiles)
            {
                SpawnGreenTile();

                yield return
                    new WaitForSeconds(5.0f);
            }

            yield return
                new WaitForSeconds(
                    spawnInterval
                );
        }
    }

    void RefreshTiles()
    {
        tiles = FindObjectsByType<Tile>();
    }

    void SpawnGreenTile()
    {
        RefreshTiles();

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
            return;

        Tile selectedTile =
            availableTiles[
                Random.Range(
                    0,
                    availableTiles.Count
                )
            ];

        selectedTile.UpdateColor(
            Tile.TileType.Target
        );

        activeGreenTiles.Add(
            selectedTile
        );

        StartCoroutine(
            RemoveGreenAfterTime(
                selectedTile
            )
        );
    }

    IEnumerator RemoveGreenAfterTime(
        Tile tile)
    {
        yield return
            new WaitForSeconds(
                greenLifetime
            );

        if (tile != null &&
            tile.tileType ==
            Tile.TileType.Target)
        {
            tile.UpdateColor(
                Tile.TileType.Normal
            );
        }

        activeGreenTiles.Remove(tile);
    }

    void RemoveInvalidTiles()
    {
        activeGreenTiles.RemoveAll(
            tile =>
                tile == null ||
                tile.tileType !=
                Tile.TileType.Target
        );
    }
}