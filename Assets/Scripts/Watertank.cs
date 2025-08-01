using UnityEngine;

public class Watertank : MonoBehaviour
{
    public float waterLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.currentTank = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetWaterLevelY()
    {
        return transform.position.y + waterLevel;
    }

    private void OnDrawGizmosSelected()
    {
        Debug .DrawLine(transform.position, new Vector3(transform.position.x, GetWaterLevelY(), transform.position.z), Color.cyan);
    }
}
