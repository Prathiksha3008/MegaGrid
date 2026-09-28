using UnityEngine;

public class MoveRight : MonoBehaviour
{
    private float moveSpeed = 5.0f;
    private float rightEdge = 5.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
        if (transform.position.x > rightEdge)
        {
            Destroy(gameObject);
        }
    }
}
