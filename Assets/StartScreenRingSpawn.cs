using UnityEngine;

public class StartScreenRingSpawn : MonoBehaviour
{
    public Ring ring;
    public GameObject[] spawnRange;
    private float spawnDelay = 1f;
    private float timer = 0f;

    private void Update()
    {
        if(timer >= spawnDelay)
        {
            timer = 0;
            float randomXPos = Random.Range(spawnRange[0].transform.position.x, spawnRange[1].transform.position.x);
            Vector3 spawnPos = new Vector3(randomXPos, spawnRange[0].transform.position.y, spawnRange[0].transform.position.z);
            Ring r = Instantiate<Ring>(ring, spawnPos, Quaternion.identity);

            r.GetComponent<Rigidbody2D>().AddTorque(Random.Range(0.5f,2));
        }
        timer += Time.deltaTime;
    }

}
