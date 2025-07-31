using UnityEngine;
using System.Collections.Generic;

public class RingManager : MonoBehaviour
{
    public GameObject ringPrefab;  // Drag your Ring prefab here in Inspector
    public Transform spawnPoint;   // Where rings spawn
    public int maxRingsInScene = 10;
    private List<GameObject> activeRings = new List<GameObject>();

    public int score = 0;
    public int combo = 0;

    void Start()
    {
        // Initial spawn
        for (int i = 0; i < maxRingsInScene; i++)
        {
            SpawnRing();
        }
    }

    public void SpawnRing()
    {
        GameObject newRing = Instantiate(ringPrefab, spawnPoint.position, Quaternion.identity);
        activeRings.Add(newRing);
    }

    public void OnRingHooked(GameObject ring)
    {
        score += 1 + combo;  // Add score with combo bonus
        combo++;

        activeRings.Remove(ring);
        Destroy(ring);

        // Respawn a new ring after a slight delay
        Invoke("SpawnRing", 0.5f);
    }

    public void OnRingMissed()  // Optional if you want to break combo on miss
    {
        combo = 0;
    }
}