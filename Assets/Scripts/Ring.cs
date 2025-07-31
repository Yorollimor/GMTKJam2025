using UnityEngine;

public class Ring : MonoBehaviour
{
    public Color[] colors;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(colors.Length > 0) GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", colors[Random.Range(0, colors.Length)]);
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
        if(force.magnitude > 0)rb.AddForce(force, ForceMode2D.Force);
    }
}
