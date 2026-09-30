using UnityEngine;

public class MoveDown : MonoBehaviour
{
    private float moveSpeed = 5.0f;
    private float bottomEdge = -2.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
        if (transform.position.z < bottomEdge)
        {
            Destroy(gameObject);
        }
    }
}
