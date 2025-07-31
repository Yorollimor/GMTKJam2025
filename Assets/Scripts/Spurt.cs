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
            UpdateRingsList();
            TriggerSpurt();
        }
    }

    void UpdateRingsList()
    {
        rings.Clear();
        rings.AddRange(FindObjectsByType<RingHandler>(FindObjectsSortMode.None));
    }

    public void TriggerSpurt()
    {
        foreach(WaterStream es in WaterStream.waterStreams)
        {
            es.StartStream(streamLength, streamStrength);
        }

        foreach (RingHandler ring in rings)
        {
            if (ring != null)
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
}
