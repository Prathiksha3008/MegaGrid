using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldenTileManager : MonoBehaviour
{
    [Header("Golden Tile Settings")]
    public float spawnDelay = 15f;

    private Tile[] tiles;
    private Tile goldenTile;

    void Start()
    {
        tiles = FindObjectsOfType<Tile>();

        StartCoroutine(GoldenTileRoutine());
    }

    IEnumerator GoldenTileRoutine()
    {
        Debug.Log(
            "Golden tile will appear in " +
            spawnDelay +
            " seconds."
        );

        yield return new WaitForSeconds(spawnDelay);

        SpawnGoldenTile();
    }

    void SpawnGoldenTile()
    {
        List<Tile> availableTiles = new List<Tile>();

        foreach (Tile tile in tiles)
        {
            if (tile.tileType == Tile.TileType.Normal)
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
            Random.Range(0, availableTiles.Count);

        goldenTile = availableTiles[randomIndex];

        goldenTile.SetTileType(Tile.TileType.Gold);

        Debug.Log("==============================");
        Debug.Log("       GOLD TILE APPEARED!");
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
        Debug.Log("==============================");
    }
}