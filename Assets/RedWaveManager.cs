using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RedWaveManager : MonoBehaviour
{
    // ==========================================
    // GRID SETTINGS
    // ==========================================

    [Header("Grid Settings")]
    public int gridSize = 9;

    // ==========================================
    // WAVE SETTINGS
    // ==========================================

    [Header("Wave Settings")]

    // Time between one completed wave and
    // the next warning.
    public float waveInterval = 0.20f;

    // Speed of red sweep.
    // Smaller = faster.
    public float tileDelay = 0.025f;

    // How long red stays after sweep finishes.
    public float waveDuration = 0.20f;

    // How long orange warning stays.
    public float warningDuration = 0.60f;

    // ==========================================
    // GAME SETTINGS
    // ==========================================

    [Header("Game Settings")]

    public float minimumGameTime = 30f;
    public float maximumGameTime = 60f;

    // ==========================================
    // UI
    // ==========================================

    [Header("UI")]

    public TextMeshProUGUI livesHUD;
    public TextMeshProUGUI timerHUD;

    // ==========================================
    // PRIVATE VARIABLES
    // ==========================================

    private Tile[] tiles;
    private PlayerMovement player;

    private List<int> currentWaveIndices =
        new List<int>();

    private bool isRowWave = true;
    private bool playerHitThisWave = false;

    private float timerNum;
    private float startingTime;

    private bool gameStopped = false;

    // ==========================================
    // PUBLIC STATE
    // ==========================================

    public bool levelCompleted = false;

    public bool GameStopped
    {
        get { return gameStopped; }
    }

    // ==========================================
    // START
    // ==========================================

    void Start()
    {
        RefreshTiles();

        player =
            FindObjectOfType<PlayerMovement>();

        timerNum =
            Random.Range(
                minimumGameTime,
                maximumGameTime
            );

        startingTime = timerNum;

        levelCompleted = false;

        if (player != null)
        {
            UpdateLives(player.lives);
        }

        UpdateTimer(timerNum);

        StartCoroutine(
            WaveRoutine()
        );
    }

    // ==========================================
    // TIMER
    // ==========================================

    void Update()
    {
        if (gameStopped)
            return;

        if (!levelCompleted)
        {
            timerNum -= Time.deltaTime;

            if (timerNum <= 0f)
            {
                timerNum = 0f;
                levelCompleted = true;

                Debug.Log(
                    "=============================="
                );

                Debug.Log(
                    "SURVIVAL COMPLETE!"
                );

                Debug.Log(
                    "GOLD TILE CAN NOW APPEAR!"
                );

                Debug.Log(
                    "=============================="
                );
            }

            UpdateTimer(timerNum);
        }
    }

    // ==========================================
    // MAIN WAVE LOOP
    // ==========================================

    IEnumerator WaveRoutine()
    {
        // Give player a moment before
        // the first warning.
        yield return new WaitForSeconds(0.5f);

        while (!gameStopped)
        {
            playerHitThisWave = false;

            // Decide row / column and
            // how many waves.
            CreateWaves();

            // Orange warning
            yield return StartCoroutine(
                FlashWarningRoutine()
            );

            if (gameStopped)
                yield break;

            // Red attack
            yield return StartCoroutine(
                SweepWave()
            );

            if (gameStopped)
                yield break;

            // Red remains briefly
            yield return new WaitForSeconds(
                waveDuration
            );

            ClearWave();

            // Very short break before
            // next warning.
            yield return new WaitForSeconds(
                waveInterval
            );
        }
    }

    // ==========================================
    // CREATE WAVES
    // ==========================================

    void CreateWaves()
    {
        ClearWave();
        RefreshTiles();

        if (player == null)
            return;

        currentWaveIndices.Clear();

        // Randomly horizontal or vertical
        isRowWave =
            Random.value > 0.5f;

        int waveCount =
            GetCurrentWaveCount();

        int playerX =
            Mathf.RoundToInt(
                player.transform.position.x
            );

        int playerZ =
            Mathf.RoundToInt(
                player.transform.position.z
            );

        int playerIndex =
            isRowWave
                ? playerZ
                : playerX;

        List<int> possibleIndices =
            new List<int>();

        for (int i = 0; i < gridSize; i++)
        {
            // Don't initially create a wave
            // directly on the player's lane.
            if (i != playerIndex)
            {
                possibleIndices.Add(i);
            }
        }

        // ==========================================
        // SHUFFLE POSSIBLE LANES
        // ==========================================

        for (
            int i = 0;
            i < possibleIndices.Count;
            i++)
        {
            int randomIndex =
                Random.Range(
                    i,
                    possibleIndices.Count
                );

            int temp =
                possibleIndices[i];

            possibleIndices[i] =
                possibleIndices[randomIndex];

            possibleIndices[randomIndex] =
                temp;
        }

        waveCount =
            Mathf.Min(
                waveCount,
                possibleIndices.Count
            );

        for (int i = 0; i < waveCount; i++)
        {
            currentWaveIndices.Add(
                possibleIndices[i]
            );
        }

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            isRowWave
                ? "RED ROW WAVES"
                : "RED COLUMN WAVES"
        );

        Debug.Log(
            "Wave Count: " +
            currentWaveIndices.Count
        );

        Debug.Log(
            "=============================="
        );
    }

    // ==========================================
    // DIFFICULTY
    // ==========================================

    int GetCurrentWaveCount()
    {
        if (startingTime <= 0f)
            return 1;

        float progress =
            1f -
            (
                timerNum /
                startingTime
            );

        // ==========================================
        // FIRST THIRD
        // 1 RED WAVE
        // ==========================================

        if (progress < 0.33f)
        {
            return 1;
        }

        // ==========================================
        // SECOND THIRD
        // 2 RED WAVES
        // ==========================================

        if (progress < 0.66f)
        {
            return 2;
        }

        // ==========================================
        // FINAL THIRD
        // 3 RED WAVES
        // ==========================================

        return 3;
    }

    // ==========================================
    // ORANGE WARNING
    // ==========================================

    IEnumerator FlashWarningRoutine()
    {
        RefreshTiles();

        foreach (Tile tile in tiles)
        {
            // ======================================
            // PROTECT GREEN AND GOLD
            // ======================================

            if (tile.tileType ==
                    Tile.TileType.Target ||
                tile.tileType ==
                    Tile.TileType.Gold)
            {
                continue;
            }

            int tileRow =
                Mathf.RoundToInt(
                    tile.transform.position.z
                );

            int tileColumn =
                Mathf.RoundToInt(
                    tile.transform.position.x
                );

            int lane =
                isRowWave
                    ? tileRow
                    : tileColumn;

            if (
                currentWaveIndices.Contains(
                    lane
                ))
            {
                Renderer renderer =
                    tile.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material.color =
                        Color.orange;
                }
            }
        }

        yield return new WaitForSeconds(
            warningDuration
        );

        // Restore original tile colors
        foreach (Tile tile in tiles)
        {
            tile.UpdateColor();
        }
    }

    // ==========================================
    // RED SWEEP
    // ==========================================

    IEnumerator SweepWave()
    {
        RefreshTiles();

        for (
            int step = 0;
            step < gridSize;
            step++)
        {
            foreach (Tile tile in tiles)
            {
                int tileRow =
                    Mathf.RoundToInt(
                        tile.transform.position.z
                    );

                int tileColumn =
                    Mathf.RoundToInt(
                        tile.transform.position.x
                    );

                int lane =
                    isRowWave
                        ? tileRow
                        : tileColumn;

                int stepPosition =
                    isRowWave
                        ? tileColumn
                        : tileRow;

                bool isWaveTile =
                    currentWaveIndices.Contains(
                        lane
                    ) &&
                    stepPosition == step;

                if (isWaveTile)
                {
                    // ==================================
                    // GREEN AND GOLD ARE PROTECTED
                    // ==================================

                    if (tile.tileType !=
                            Tile.TileType.Target &&
                        tile.tileType !=
                            Tile.TileType.Gold)
                    {
                        Renderer renderer =
                            tile.GetComponent<Renderer>();

                        if (renderer != null)
                        {
                            renderer.material.color =
                                Color.red;
                        }
                    }
                }
            }

            // Check if the player gets hit
            CheckPlayer(step);

            yield return new WaitForSeconds(
                tileDelay
            );
        }
    }

    // ==========================================
    // CHECK PLAYER
    // ==========================================

    void CheckPlayer(int currentStep)
    {
        if (player == null)
            return;

        if (playerHitThisWave)
            return;

        int playerX =
            Mathf.RoundToInt(
                player.transform.position.x
            );

        int playerZ =
            Mathf.RoundToInt(
                player.transform.position.z
            );

        int playerLane =
            isRowWave
                ? playerZ
                : playerX;

        int playerStepPosition =
            isRowWave
                ? playerX
                : playerZ;

        // Player is not on one of
        // the active red lanes.
        if (
            !currentWaveIndices.Contains(
                playerLane
            ))
        {
            return;
        }

        // IMPORTANT:
        // Only hit the player when the actual
        // red sweep reaches their tile.
        if (playerStepPosition != currentStep)
        {
            return;
        }

        // ==========================================
        // CHECK PLAYER TILE
        // ==========================================

        Tile playerTile =
            GetTileAt(
                playerX,
                playerZ
            );

        if (playerTile != null)
        {
            // ======================================
            // GREEN = SAFE
            // GOLD = SAFE
            // ======================================

            if (playerTile.tileType ==
                    Tile.TileType.Target ||
                playerTile.tileType ==
                    Tile.TileType.Gold)
            {
                return;
            }
        }

        // ==========================================
        // PLAYER HIT
        // ==========================================

        playerHitThisWave = true;

        player.lives--;

        UpdateLives(
            player.lives
        );

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "RED WAVE HIT!"
        );

        Debug.Log(
            "Lives remaining: " +
            player.lives
        );

        Debug.Log(
            "=============================="
        );

        // ==========================================
        // GAME OVER
        // ==========================================

        if (player.lives <= 0)
        {
            StopGame();

            player.GameOver();
        }
    }

    // ==========================================
    // FIND TILE AT GRID POSITION
    // ==========================================

    Tile GetTileAt(
        int x,
        int z)
    {
        if (tiles == null)
            return null;

        foreach (Tile tile in tiles)
        {
            int tileX =
                Mathf.RoundToInt(
                    tile.transform.position.x
                );

            int tileZ =
                Mathf.RoundToInt(
                    tile.transform.position.z
                );

            if (
                tileX == x &&
                tileZ == z)
            {
                return tile;
            }
        }

        return null;
    }

    // ==========================================
    // CLEAR WAVE
    // ==========================================

    void ClearWave()
    {
        RefreshTiles();

        if (tiles == null)
            return;

        foreach (Tile tile in tiles)
        {
            tile.UpdateColor();
        }
    }

    // ==========================================
    // STOP GAME
    // ==========================================

    public void StopGame()
    {
        if (gameStopped)
            return;

        gameStopped = true;

        StopAllCoroutines();

        ClearWave();
    }

    // ==========================================
    // UI
    // ==========================================

    void UpdateLives(int num)
    {
        if (livesHUD != null)
        {
            livesHUD.text =
                "Lives: " + num;
        }
    }

    void UpdateTimer(float num)
    {
        if (timerHUD != null)
        {
            timerHUD.text =
                "Timer: " +
                Mathf.CeilToInt(num);
        }
    }

    // ==========================================
    // REFRESH TILES
    // ==========================================

    void RefreshTiles()
    {
        tiles =
            FindObjectsOfType<Tile>();
    }
}