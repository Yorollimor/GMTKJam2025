using UnityEngine;

public class Ring : MonoBehaviour
{
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 force = Vector3.zero;
        foreach (WaterStream es in WaterStream.waterStreams)
        {
            force += FindAnyObjectByType<WaterStream>().GetMoveVector(rb.position);
        }

        force /= WaterStream.waterStreams.Count;
        rb.AddForce(force, ForceMode2D.Force);
    }
}
