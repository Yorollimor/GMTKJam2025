using UnityEngine;

public class UIComboParticlesSpawner : MonoBehaviour
{
    [Header("Assign the particle prefab (UI-compatible)")]
    public GameObject comboParticlePrefab;

    [Header("Optional: Parent it under the same canvas")]
    public Transform particleParent;

    [Header("Reference to the UI combo text")]
    public RectTransform comboTextUI;

    public void SpawnComboParticles()
    {
        if (comboParticlePrefab == null || comboTextUI == null)
        {
            Debug.LogWarning("Combo particle prefab or combo text UI is not assigned!");
            return;
        }

        // Instantiate inside the UI
        GameObject particles = Instantiate(comboParticlePrefab, comboTextUI.position, Quaternion.identity);

        // If we have a parent, make it a child
        if (particleParent != null)
            particles.transform.SetParent(particleParent, worldPositionStays: true);

        // Destroy after particle duration
        var ps = particles.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(particles, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(particles, 2f); // fallback
        }
    }
}