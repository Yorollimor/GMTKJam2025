using UnityEngine;

public class Watertank : MonoBehaviour
{
    public float moveRange;
    public Vector2 lowestPoint = new Vector2(-1f, -1f);

    public float waterLevel;

    Vector3 startPoint;

    bool isGrabed = false;
    Vector2 grabPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPoint = transform.position;
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

    public void IsGrabed()
    {
        isGrabed = true;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        grabPoint = transform.InverseTransformPoint(mouseWorldPos);
        Debug.Log($"Grabbed at {grabPoint}");
    }
    
    public void IsReleased()
    {
        isGrabed = false;
    }

    public void MoveGrabbed()
    {
        if (isGrabed)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 grabPointWorld = transform.TransformPoint(grabPoint);

            Vector3 newPos = grabPointWorld - mouseWorldPos;
            newPos.x = Mathf.Clamp(newPos.x, startPoint.x - moveRange, startPoint.x + moveRange);
            newPos.y = transform.position.y;
            transform.position = newPos;
            Debug.Log($"Moving to {newPos}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, GetWaterLevelY(), transform.position.z));
        Gizmos.DrawLine(transform.position + moveRange * Vector3.left, transform.position + moveRange * Vector3.right);
        Gizmos.DrawLine(transform.position + lowestPoint.x * Vector3.left - lowestPoint.y * Vector3.down, transform.position + lowestPoint.x * Vector3.right - lowestPoint.y * Vector3.down);
    }
}
