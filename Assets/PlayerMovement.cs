using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 1.2f;

    public int lives = 3;
    public int score = 0;

    private bool canMove = true;
    private bool gameWon = false;

    void Update()
    {
        if (!canMove || gameWon)
            return;

        // ==========================================
        // JUMP
        // Direction + SPACE = 2 TILES
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

            // Space alone does nothing
            return;
        }

        // ==========================================
        // NORMAL MOVEMENT
        // Direction only = 1 TILE
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

    // ==========================================
    // TRY MOVE
    // ==========================================

    void TryMove(
        Vector3 direction,
        int tiles,
        bool isJump
    )
    {
        Vector3 startPosition = transform.position;

        Vector3 targetPosition =
            startPosition + direction * tiles;

        // ==========================================
        // GRID BOUNDARIES
        // ==========================================

        if (targetPosition.x < 0 ||
            targetPosition.x > 4 ||
            targetPosition.z < 0 ||
            targetPosition.z > 4)
        {
            Debug.Log("CANNOT MOVE OUTSIDE THE GRID!");
            return;
        }

        // ==========================================
        // DEBUG
        // ==========================================

        if (isJump)
        {
            Debug.Log(
                "JUMP: (" +
                Mathf.RoundToInt(startPosition.x) +
                "," +
                Mathf.RoundToInt(startPosition.z) +
                ") → (" +
                Mathf.RoundToInt(targetPosition.x) +
                "," +
                Mathf.RoundToInt(targetPosition.z) +
                ")"
            );

            StartCoroutine(
                JumpToPosition(targetPosition)
            );
        }
        else
        {
            Debug.Log(
                "MOVE: (" +
                Mathf.RoundToInt(startPosition.x) +
                "," +
                Mathf.RoundToInt(startPosition.z) +
                ") → (" +
                Mathf.RoundToInt(targetPosition.x) +
                "," +
                Mathf.RoundToInt(targetPosition.z) +
                ")"
            );

            StartCoroutine(
                MoveToPosition(targetPosition)
            );
        }
    }

    // ==========================================
    // NORMAL MOVEMENT
    // ==========================================

    IEnumerator MoveToPosition(
        Vector3 targetPosition
    )
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

        // Only check the LANDING tile
        CheckForGoal();

        canMove = true;
    }

    // ==========================================
    // JUMP MOVEMENT
    // ==========================================

    IEnumerator JumpToPosition(
        Vector3 targetPosition
    )
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

            // Horizontal movement
            Vector3 position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            // Parabolic jump arc
            float arc =
                Mathf.Sin(t * Mathf.PI) *
                jumpHeight;

            position.y += arc;

            transform.position =
                position;

            yield return null;
        }

        // ==========================================
        // LAND DIRECTLY ON TARGET TILE
        // ==========================================

        transform.position =
            targetPosition;

        // Only landing position matters
        CheckForGoal();

        canMove = true;
    }

    // ==========================================
    // GOAL
    // ==========================================

    void CheckForGoal()
    {
        int playerX =
            Mathf.RoundToInt(
                transform.position.x
            );

        int playerZ =
            Mathf.RoundToInt(
                transform.position.z
            );

        if (playerX == 0 &&
            playerZ == 4)
        {
            WinGame();
        }
    }

    // ==========================================
    // WIN
    // ==========================================

    public void WinGame()
    {
        gameWon = true;

        Debug.Log("==============================");
        Debug.Log("          YOU WIN!");
        Debug.Log("       GOAL REACHED!");
        Debug.Log("==============================");
        StartCoroutine(WaitRoutine(10));
        SceneManager.LoadScene("SampleScene");
    }
    private IEnumerator WaitRoutine(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}