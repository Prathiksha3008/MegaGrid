using UnityEngine;
using TMPro; // Needed to update the Lives UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Needed to handle scenes like restarting a level

public class RedWaveManager : MonoBehaviour
{
    // ==========================================
    // WAVE SETTINGS
    // ==========================================

    public float waveInterval = 1.5f;

    // How quickly the red wave fills its row/column
    public float tileDelay = 0.08f;

    // How long the complete wave remains visible
    public float waveDuration = 0.5f;

    private Tile[] tiles;
    private PlayerMovement player;

    private int currentWaveIndex = -1;
    private bool isRowWave = true;

    private bool playerHitThisWave = false;
    public TextMeshProUGUI livesHUD, timerHUD;
    private float timerNum;
    public bool levelCompleted; // Return true if the player survives after the timer hits zero, and false if not.

    void Start()
    {
        tiles = FindObjectsOfType<Tile>();
        player = FindObjectOfType<PlayerMovement>();
        timerNum = Random.Range(30, 61);
        updateLives(player.lives);
        updateTimer(timerNum);
        levelCompleted = false;
        StartCoroutine(WaveRoutine());
    }

    private void FixedUpdate()
    {
        timerNum -= Time.deltaTime;
        updateTimer(timerNum);
        if (timerNum <= 0)
        {
            updateTimer(0);
            levelCompleted = true;
        }
    }

    // ==========================================
    // MAIN WAVE LOOP
    // ==========================================

    IEnumerator WaveRoutine()
    {
        while (true)
        {
            // Wait before creating the next wave
            yield return new WaitForSeconds(waveInterval);

            playerHitThisWave = false;

            CreateWave();

            yield return StartCoroutine(FlashWarningRoutine());

            yield return StartCoroutine(SweepWave());

            // Keep the complete wave visible
            yield return new WaitForSeconds(waveDuration);

            ClearWave();
        }
    }

    // ==========================================
    // CREATE WAVE BASED ON PLAYER POSITION
    // ==========================================

    void CreateWave()
    {
        ClearWave();

        if (player == null)
            return;

        int playerX =
            Mathf.RoundToInt(
                player.transform.position.x
            );

        int playerZ =
            Mathf.RoundToInt(
                player.transform.position.z
            );

        // ==========================================
        // RANDOMLY CHOOSE ROW OR COLUMN
        // ==========================================

        isRowWave = Random.value > 0.5f;

        // ==========================================
        // ROW WAVE
        // ==========================================

        if (isRowWave)
        {
            List<int> possibleRows =
                new List<int>();

            for (int row = 0; row < 5; row++)
            {
                // Never create the wave directly
                // on the player's current row
                if (row != playerZ)
                {
                    possibleRows.Add(row);
                }
            }

            // Put closest rows first
            possibleRows.Sort(
                (a, b) =>
                    Mathf.Abs(a - playerZ)
                    .CompareTo(
                        Mathf.Abs(b - playerZ)
                    )
            );

            // Choose one of the closest two
            int choices =
                Mathf.Min(
                    2,
                    possibleRows.Count
                );

            currentWaveIndex =
                possibleRows[
                    Random.Range(0, choices)
                ];

            Debug.Log(
                "=============================="
            );

            Debug.Log(
                "RED ROW WAVE"
            );

            Debug.Log(
                "Wave Row: " +
                (currentWaveIndex + 1)
            );

            Debug.Log(
                "Player Position: (" +
                (playerX + 1) +
                "," +
                (playerZ + 1) +
                ")"
            );

            Debug.Log(
                "=============================="
            );
        }

        // ==========================================
        // COLUMN WAVE
        // ==========================================

        else
        {
            List<int> possibleColumns =
                new List<int>();

            for (int column = 0; column < 5; column++)
            {
                // Never create the wave directly
                // on the player's current column
                if (column != playerX)
                {
                    possibleColumns.Add(column);
                }
            }

            // Put closest columns first
            possibleColumns.Sort(
                (a, b) =>
                    Mathf.Abs(a - playerX)
                    .CompareTo(
                        Mathf.Abs(b - playerX)
                    )
            );

            // Choose one of the closest two
            int choices =
                Mathf.Min(
                    2,
                    possibleColumns.Count
                );

            currentWaveIndex =
                possibleColumns[
                    Random.Range(0, choices)
                ];

            Debug.Log(
                "=============================="
            );

            Debug.Log(
                "RED COLUMN WAVE"
            );

            Debug.Log(
                "Wave Column: " +
                (currentWaveIndex + 1)
            );

            Debug.Log(
                "Player Position: (" +
                (playerX + 1) +
                "," +
                (playerZ + 1) +
                ")"
            );

            Debug.Log(
                "=============================="
            );
        }
    }

    // ==========================================
    // SWEEP WAVE ACROSS 5 TILES
    // ==========================================

    IEnumerator SweepWave()
    {
        for (int step = 0; step < 5; step++)
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

                bool isWaveTile = false;

                // ROW
                if (isRowWave)
                {
                    isWaveTile =
                        tileRow == currentWaveIndex &&
                        tileColumn == step;
                }

                // COLUMN
                else
                {
                    isWaveTile =
                        tileColumn == currentWaveIndex &&
                        tileRow == step;
                }

                if (isWaveTile)
                {
                    tile.GetComponent<Renderer>().material.color =
                        Color.red;
                }
            }

            // ==========================================
            // CHECK PLAYER WHILE WAVE IS APPEARING
            // ==========================================

            CheckPlayer();

            yield return new WaitForSeconds(
                tileDelay
            );
        }
    }

    // ==========================================
    // CHECK PLAYER POSITION
    // ==========================================

    void CheckPlayer()
    {
        if (player == null)
            return;

        // Don't remove multiple lives from
        // the same wave
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

        bool playerOnWave = false;

        // ==========================================
        // ROW WAVE
        // ==========================================

        if (isRowWave)
        {
            if (playerZ == currentWaveIndex)
            {
                playerOnWave = true;
            }
        }

        // ==========================================
        // COLUMN WAVE
        // ==========================================

        else
        {
            if (playerX == currentWaveIndex)
            {
                playerOnWave = true;
            }
        }

        // ==========================================
        // PLAYER HIT
        // ==========================================

        if (playerOnWave)
        {
            playerHitThisWave = true;

            player.lives--;
            updateLives(player.lives);

            Debug.Log(
                "RED WAVE HIT!"
            );

            Debug.Log(
                "Lives remaining: " +
                player.lives
            );

            // If the player gets a game over, pause the game for some seconds, then reload the scene.
            if (player.lives <= 0)
            {
                Debug.Log(
                    "GAME OVER!"
                );
                StartCoroutine(WaitRoutine(10));
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    // ==========================================
    // CLEAR WAVE
    // ==========================================

    void ClearWave()
    {
        if (tiles == null)
            return;

        foreach (Tile tile in tiles)
        {
            if (tile.tileType ==
                Tile.TileType.Target)
            {
                tile.GetComponent<Renderer>().material.color =
                    Color.green;
            }
            else
            {
                tile.GetComponent<Renderer>().material.color =
                    Color.blue;
            }
        }
    }

    private void updateLives(int num)
    {
        livesHUD.text = "Lives: " + num;
    }

    private void updateTimer(float num)
    {
        timerHUD.text = "Timer: " + num;
    }

    private IEnumerator WaitRoutine(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private IEnumerator FlashWarningRoutine()
    {
        for (int step = 0; step < 5; step++)
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

                bool isWaveTile = false;

                // ROW
                if (isRowWave)
                {
                    isWaveTile =
                        tileRow == currentWaveIndex &&
                        tileColumn == step;
                }

                // COLUMN
                else
                {
                    isWaveTile =
                        tileColumn == currentWaveIndex &&
                        tileRow == step;
                }

                if (isWaveTile)
                {
                    tile.GetComponent<Renderer>().material.color =
                        Color.orange;
                }
            }
        }
        yield return new WaitForSeconds(2.5f);
    }
}