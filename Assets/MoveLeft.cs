using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float moveSpeed = 5.0f;
    private float leftEdge = -2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
