using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public int lives = 3;
    public int score = 0;

    private bool canMove = true;
    private bool gameStarted = false;
    private bool gameWon = false;

    public void BeginGame()
    {
        gameStarted = true;
    }

    void Update()
    {
        if (!gameStarted || !canMove || gameWon)
            return;

        // ==========================================
        // FORWARD
        // ==========================================

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            TryMove(Vector3.forward);
            return;
        }

        // ==========================================
        // BACK
        // ==========================================

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            TryMove(Vector3.back);
            return;
        }

        // ==========================================
        // LEFT
        // ==========================================

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            TryMove(Vector3.left);
            return;
        }

        // ==========================================
        // RIGHT
        // ==========================================

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            TryMove(Vector3.right);
            return;
        }
    }

    // ==========================================
    // MOVE ONE TILE
    // ==========================================

    void TryMove(Vector3 direction)
    {
        Vector3 startPosition =
            transform.position;

        Vector3 targetPosition =
            startPosition + direction;

        // ==========================================
        // GRID BOUNDARIES
        // ==========================================

        if (targetPosition.x < 0 ||
            targetPosition.x > 4 ||
            targetPosition.z < 0 ||
            targetPosition.z > 4)
        {
            Debug.Log(
                "CANNOT MOVE OUTSIDE THE GRID!"
            );

            return;
        }

        StartCoroutine(
            MoveToPosition(targetPosition)
        );
    }

    // ==========================================
    // MOVE ANIMATION
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

        CheckForGoal();

        canMove = true;
    }

    // ==========================================
    // CHECK GOAL
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

    void WinGame()
    {
        gameWon = true;

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "YOU WIN!"
        );

        Debug.Log(
            "GOAL REACHED!"
        );

        Debug.Log(
            "=============================="
        );
    }
}