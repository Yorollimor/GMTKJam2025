using UnityEngine;

public class Srudl : MonoBehaviour
{
    public float Strength = 1.0f;
    public float MaxDist = 1.0f;
    public float MaxTorque = 1.0f;
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
            Sprudl();
        }
    }

    void Sprudl()
    {
        foreach (Ring ring in rings)
        {
            Vector3 dist = ring.transform.position - transform.position;
            float strength = Mathf.Clamp01(1.0f - dist.magnitude / MaxDist) * Strength;
            strength *= strength;
            strength *= Vector3.Dot(dist.normalized, Vector3.up); // Ensure positive influence
            ring.GetComponent<Rigidbody2D>().AddForce(strength * dist.normalized, ForceMode2D.Impulse);
            ring.GetComponent<Rigidbody2D>().AddTorque(strength * Random.Range(-MaxTorque, MaxTorque), ForceMode2D.Impulse);
        }
    }

    
}
