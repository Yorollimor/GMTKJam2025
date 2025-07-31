using System.Collections.Generic;
using UnityEngine;

public class Srudl : MonoBehaviour
{
    public float Strength = 1.0f;
    public float MaxDist = 1.0f;
    public float MaxTorque = 1.0f;

    public float streamLength, streamStrength;

    List<RingHandler> rings = new List<RingHandler>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateRingsList();
            Sprudl();
        }
    }

    void UpdateRingsList()
    {
        rings.Clear();
        rings.AddRange(FindObjectsOfType<RingHandler>());
    }

    void Sprudl()
    {
        foreach(WaterStream es in WaterStream.waterStreams)
        {
            es.StartStream(streamLength, streamStrength);
        }

        foreach (RingHandler ring in rings)
        {
            if (ring != null)
            {
                Vector3 dist = ring.transform.position - transform.position;
                float strength = Mathf.Clamp01(1.0f - dist.magnitude / MaxDist) * Strength;
                strength *= strength;
                strength *= Vector3.Dot(dist.normalized, Vector3.up); // Ensure positive influence

                Rigidbody2D rb = ring.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(strength * dist.normalized, ForceMode2D.Impulse);
                    rb.AddTorque(strength * Random.Range(-MaxTorque, MaxTorque), ForceMode2D.Impulse);
                }
            }
        }
    }
}
