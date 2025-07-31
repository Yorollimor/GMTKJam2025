using UnityEngine;

public class Spurt : MonoBehaviour
{
    public float Strength = 1.0f;
    public float MaxDist = 1.0f;
    public float MaxTorque = 1.0f;

    public float streamLength, streamStrength;

    Ring[] rings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rings = FindObjectsByType<Ring>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerSpurt();
        }
    }

    public void TriggerSpurt()
    {
        foreach(WaterStream es in WaterStream.waterStreams)
        {
            es.StartStream(streamLength, streamStrength);
        }
        

        foreach (Ring ring in rings)
        {
            Vector3 direction = ring.transform.position - transform.position;
            float strength = Mathf.Clamp01(1.0f - direction.magnitude / MaxDist) * Strength;
            strength *= strength;
            strength *= Vector3.Dot(direction.normalized, Vector3.up); // Ensure positive influence
            direction = (direction.normalized + transform.up) / 2;
            ring.GetComponent<Rigidbody2D>().AddForce(strength * direction.normalized, ForceMode2D.Impulse);
            ring.GetComponent<Rigidbody2D>().AddTorque(strength * Random.Range(-MaxTorque, MaxTorque), ForceMode2D.Impulse);
        }
    }

    
}
