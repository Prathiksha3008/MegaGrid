using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.2f;

    [Header("Player")]
    public int lives = 3;
    public int score = 0;

    [Header("Grid")]
    public int gridSize = 9;

    private bool canMove = true;
    private bool gameWon = false;
    private bool gameOver = false;

    private RedWaveManager redWaveManager;

    void Start()
    {
        redWaveManager =
            FindObjectOfType<RedWaveManager>();
    }

    void Update()
    {
        if (!canMove || gameWon || gameOver)
            return;

        // ==========================================
        // JUMP
        // Direction + Space = 2 Tiles
        // ==========================================

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (Keyboard.current.wKey.isPressed ||
                Keyboard.current.upArrowKey.isPressed)
            {
                TryMove(Vector3.forward, 2, true);
                return;
            }

            if (Keyboard.current.sKey.isPressed ||
                Keyboard.current.downArrowKey.isPressed)
            {
                TryMove(Vector3.back, 2, true);
                return;
            }

            if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            {
                TryMove(Vector3.left, 2, true);
                return;
            }

            if (Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed)
            {
                TryMove(Vector3.right, 2, true);
                return;
            }

            return;
        }

        // ==========================================
        // NORMAL MOVEMENT
        // ==========================================

        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            TryMove(Vector3.forward, 1, false);
            return;
        }

        if (Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            TryMove(Vector3.back, 1, false);
            return;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame ||
            Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            TryMove(Vector3.left, 1, false);
            return;
        }

        if (Keyboard.current.dKey.wasPressedThisFrame ||
            Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            TryMove(Vector3.right, 1, false);
            return;
        }
    }

    void TryMove(
        Vector3 direction,
        int tiles,
        bool isJump)
    {
        Vector3 startPosition =
            transform.position;

        Vector3 targetPosition =
            startPosition + direction * tiles;

        // ==========================================
        // 9x9 GRID BOUNDARIES
        // ==========================================

        if (targetPosition.x < 0 ||
            targetPosition.x > gridSize - 1 ||
            targetPosition.z < 0 ||
            targetPosition.z > gridSize - 1)
        {
            Debug.Log(
                "CANNOT MOVE OUTSIDE THE GRID!"
            );

            return;
        }

        if (isJump)
        {
            StartCoroutine(
                JumpToPosition(targetPosition)
            );
        }
        else
        {
            StartCoroutine(
                MoveToPosition(targetPosition)
            );
        }
    }

    IEnumerator MoveToPosition(
        Vector3 targetPosition)
    {
        canMove = false;

        Vector3 startPosition =
            transform.position;

        float distance =
            Vector3.Distance(
                startPosition,
                targetPosition
            );

        float duration =
            distance / moveSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            yield return null;
        }

        transform.position =
            targetPosition;

        CheckForGoal();

        canMove = true;
    }

    IEnumerator JumpToPosition(
        Vector3 targetPosition)
    {
        canMove = false;

        Vector3 startPosition =
            transform.position;

        float distance =
            Vector3.Distance(
                startPosition,
                targetPosition
            );

        float duration =
            distance / moveSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            Vector3 position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            float arc =
                Mathf.Sin(t * Mathf.PI) *
                jumpHeight;

            position.y += arc;

            transform.position = position;

            yield return null;
        }

        transform.position =
            targetPosition;

        CheckForGoal();

        canMove = true;
    }

    // ==========================================
    // GOLD TILE CHECK
    // ==========================================

    void CheckForGoal()
    {
        if (redWaveManager == null)
            return;

        if (!redWaveManager.levelCompleted)
            return;

        int playerX =
            Mathf.RoundToInt(
                transform.position.x
            );

        int playerZ =
            Mathf.RoundToInt(
                transform.position.z
            );

        Tile[] allTiles =
            FindObjectsOfType<Tile>();

        foreach (Tile tile in allTiles)
        {
            int tileX =
                Mathf.RoundToInt(
                    tile.transform.position.x
                );

            int tileZ =
                Mathf.RoundToInt(
                    tile.transform.position.z
                );

            if (tileX == playerX &&
                tileZ == playerZ &&
                tile.tileType ==
                Tile.TileType.Gold)
            {
                WinGame();
                return;
            }
        }
    }

    // ==========================================
    // WIN
    // ==========================================

    public void WinGame()
    {
        if (gameWon || gameOver)
            return;

        gameWon = true;
        canMove = false;

        Debug.Log(
            "=============================="
        );

        Debug.Log("YOU WIN!");

        Debug.Log(
            "GOLD TILE REACHED!"
        );

        Debug.Log(
            "=============================="
        );

        if (redWaveManager != null)
        {
            redWaveManager.StopGame();
        }

        StartCoroutine(
            RestartAfterDelay()
        );
    }

    // ==========================================
    // GAME OVER
    // ==========================================

    public void GameOver()
    {
        if (gameOver || gameWon)
            return;

        gameOver = true;
        canMove = false;

        Debug.Log(
            "=============================="
        );

        Debug.Log("GAME OVER!");

        Debug.Log(
            "=============================="
        );

        StartCoroutine(
            RestartAfterDelay()
        );
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}