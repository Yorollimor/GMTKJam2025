using UnityEngine;
using System.Collections.Generic;

public class RingManager : MonoBehaviour
{
    public GameObject ringPrefab;  // Drag your Ring prefab here in Inspector
    public int maxRingsInScene = 10;
    private List<GameObject> activeRings = new List<GameObject>();

    public int score = 0;
    public int combo = 0;

    public float spawnDelay = 1f;
    private float spawnTimer = 0f;

    private void Start()
    {
        GameManager.Instance.ringManager = this;  
    }
    private void Update()
    {
        // Initial spawn
        if (spawnTimer >= spawnDelay && activeRings.Count < maxRingsInScene)
        {
            spawnTimer = 0;
            SpawnRing();
        }
        spawnTimer += Time.deltaTime;
    }

    public void SpawnRing()
    {
        Transform spawnPoint = GameManager.Instance.currentTank.GetRandomSpawnPoint();
        GameObject newRing = Instantiate(ringPrefab, spawnPoint.position, Quaternion.identity);
        activeRings.Add(newRing);
    }

    public void RingDestroyed(GameObject ring)
    {
        activeRings.Remove(ring);
    }

    public void OnRingMissed()  // Optional if you want to break combo on miss
    {
        combo = 0;
    }
}