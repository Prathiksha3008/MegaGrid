using UnityEngine;

public class MoveUp : MonoBehaviour
{
    private float moveSpeed = 5.0f;
    private float topEdge = 4.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        if (transform.position.z > topEdge)
        {
            Destroy(gameObject);
        }
    }
}
