using System.Collections.Generic;
using UnityEngine;

public class Spurt : MonoBehaviour
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
<<<<<<< HEAD:Assets/Scripts/Srudl.cs
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
=======
            TriggerSpurt();
        }
    }

    public void TriggerSpurt()
>>>>>>> dc8e1a1a7f3e31e8e407c3c548fb6fb417656443:Assets/Scripts/Spurt.cs
    {
        foreach(WaterStream es in WaterStream.waterStreams)
        {
            es.StartStream(streamLength, streamStrength);
        }

        foreach (RingHandler ring in rings)
        {
<<<<<<< HEAD:Assets/Scripts/Srudl.cs
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
=======
            Vector3 direction = ring.transform.position - transform.position;
            float strength = Mathf.Clamp01(1.0f - direction.magnitude / MaxDist) * Strength;
            strength *= strength;
            strength *= Vector3.Dot(direction.normalized, Vector3.up); // Ensure positive influence
            direction = (direction.normalized + transform.up) / 2;
            ring.GetComponent<Rigidbody2D>().AddForce(strength * direction.normalized, ForceMode2D.Impulse);
            ring.GetComponent<Rigidbody2D>().AddTorque(strength * Random.Range(-MaxTorque, MaxTorque), ForceMode2D.Impulse);
>>>>>>> dc8e1a1a7f3e31e8e407c3c548fb6fb417656443:Assets/Scripts/Spurt.cs
        }
    }
}
