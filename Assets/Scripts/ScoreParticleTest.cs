using UnityEngine;

public class ScoreParticleTest : MonoBehaviour
{
    public RectTransform uiTarget; // Your score or UI element
    public GameObject particlePrefab; // The particle prefab

    void Update()
    {
        // Press space to spawn particles at UI position
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnParticlesAtUI();
        }
    }

    void SpawnParticlesAtUI()
    {
        if (uiTarget == null)
        {
            Debug.LogWarning("⚠ uiTarget is not assigned!");
            return;
        }

        if (particlePrefab == null)
        {
            Debug.LogWarning("⚠ particlePrefab is not assigned!");
            return;
        }

        // Step 1: Get screen position of UI element
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiTarget.position);
        Debug.Log($"📍 Screen Position of UI: {screenPos}");

        // Step 2: Convert screen position to world space
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        Debug.Log($"🌍 World Position for particle: {worldPos}");

        // Step 3: Spawn particle
        GameObject particle = Instantiate(particlePrefab, worldPos, Quaternion.identity);
        var ps = particle.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        Debug.Log($"✨ Spawned particle: {particle.name}");
    }
}