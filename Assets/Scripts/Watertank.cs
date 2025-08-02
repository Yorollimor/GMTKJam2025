using JetBrains.Annotations;
using UnityEngine;

public class Watertank : MonoBehaviour
{
    public float moveRange;
    public Vector2 lowestPoint = new Vector2(-1f, -1f);
    public float maxAngle = 45f;

    public float waterLevel;

    Vector2 startPoint;

    bool isGrabed = false;
    Vector2 grabPoint;

    Vector2 targetPos;
    float targetRot;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    Vector2 velocity;
    public float velocityDragMultiplier = 0.95f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPoint = targetPos = transform.position;
        targetRot = transform.rotation.eulerAngles.z;
        rb = GetComponentInChildren<Rigidbody2D>();
        GameManager.Instance.currentTank = this;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if(Input.GetKey(KeyCode.D)) targetPos.x += moveSpeed * Time.deltaTime;
            else targetPos.x -= moveSpeed * Time.deltaTime;
            targetPos.x = Mathf.Clamp(targetPos.x, startPoint.x - moveRange, startPoint.x + moveRange);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Vector2.Distance(transform.position, targetPos) > 0.001f)
        {
            Vector3 prevPos = transform.position;
            transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
            velocity = (transform.position - prevPos) / Time.deltaTime;
        }
    }

    public float GetWaterLevelY()
    {
        return transform.position.y + waterLevel;
    }

    public void IsGrabed()
    {
        isGrabed = true; 
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z; // For 2D

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        grabPoint = transform.InverseTransformPoint(mouseWorldPos);

        Debug.Log($"Grabbed at {grabPoint} mouse at {mouseWorldPos} transform {transform.InverseTransformPoint(mouseWorldPos)} mp {Input.mousePosition}");
    }
    
    public void IsReleased()
    {
        isGrabed = false;
    }

    public void MoveGrabbed()
    {
        if (isGrabed)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = -Camera.main.transform.position.z;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector3 newWorldPos = mouseWorldPos - transform.TransformVector(grabPoint);
            newWorldPos.y = transform.position.y; // Keep the water level Y
            newWorldPos.x = Mathf.Clamp(newWorldPos.x, startPoint.x - moveRange, startPoint.x + moveRange);
            targetPos = newWorldPos;


            return;

            Vector3 mouseDir = mouseWorldPos - new Vector3(transform.position.x, startPoint.y, mouseWorldPos.z);
            mouseDir.x = Mathf.Abs(mouseDir.x);
            float angle = Vector3.Angle(Vector3.right, mouseDir);
            angle = Mathf.Clamp(angle, 0, maxAngle);

            Vector3 pivot = lowestPoint;
            if (grabPoint.x > 0) lowestPoint.x = Mathf.Abs(lowestPoint.x);
            else lowestPoint.x = -Mathf.Abs(lowestPoint.x);

            pivot = transform.TransformPoint(pivot);

            // Move pivot to world space
            Vector3 dir = transform.position - pivot;

            // Rotate direction vector
            dir = Quaternion.Euler(0, 0, angle) * dir;

            // Compute new position
            targetPos = pivot + dir;

            // Apply rotation
            targetRot = angle;
        }
    }

    public Vector2 GetTankVelocity()
    {
        return velocity * velocityDragMultiplier;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, GetWaterLevelY(), transform.position.z));
        Gizmos.DrawLine(transform.position + moveRange * Vector3.left, transform.position + moveRange * Vector3.right);
        Gizmos.DrawLine(transform.position + lowestPoint.x * Vector3.left - lowestPoint.y * Vector3.down, transform.position + lowestPoint.x * Vector3.right - lowestPoint.y * Vector3.down);
    }
}
