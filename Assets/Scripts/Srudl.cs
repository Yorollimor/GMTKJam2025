using UnityEngine;

public class Srudl : MonoBehaviour
{
    public float Strength = 1.0f;
    public float MaxDist = 1.0f;
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
            Vector3 randomVec = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            );
            float strength = Mathf.Clamp01(1.0f - dist.magnitude / MaxDist) * Strength;
            strength *= strength;
            strength *= Vector3.Dot(dist.normalized, Vector3.up); // Ensure positive influence
            ring.GetComponent<Rigidbody>().AddForce(strength * dist.normalized, ForceMode.Impulse);
            ring.GetComponent<Rigidbody>().AddTorque(strength * randomVec, ForceMode.Impulse);
        }
    }

    
}
