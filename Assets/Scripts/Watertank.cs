
using TMPro;
using UnityEngine;

public enum TankState
{
    Moving,
    Reverse,
    Stop
}

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
    public float moveSpeed = 5f, keyboardSpeed;
    public Rigidbody2D watertankPhysics;
    public Transform watertankVisuals;
    public Transform spawnPointParent;
    private Transform[] spawnPoints;

    public TextMeshPro scoreText;
    public Transform moveableObjectsParent;

    Vector2 velocity;
    Vector3 prevPos;
    public float velocityDragMultiplier = 0.95f;

    private InteractableCollider2D[] interactables;

    FMOD.Studio.EventInstance soundInstance;
    TankState soundState = TankState.Stop;
    public float maxSoundVelocity;

    private bool turnedOffInteractions = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPoint = targetPos = watertankPhysics.transform.position;
        targetRot = watertankPhysics.transform.rotation.eulerAngles.z;
        GameManager.Instance.currentTank = this;

        spawnPoints = spawnPointParent.GetComponentsInChildren<Transform>();
        interactables = GetComponentsInChildren<InteractableCollider2D>(true);
    }

    private void Update()
    {
        if (turnedOffInteractions) return;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if(Input.GetKey(KeyCode.D)) targetPos.x += keyboardSpeed * Time.deltaTime;
            else targetPos.x -= keyboardSpeed * Time.deltaTime;
            targetPos.x = Mathf.Clamp(targetPos.x, startPoint.x - moveRange, startPoint.x + moveRange);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector2.Distance(watertankPhysics.transform.position, targetPos) > 0.001f)
        {

            watertankPhysics.MovePosition(Vector2.Lerp(watertankPhysics.transform.position, targetPos, Time.deltaTime * moveSpeed));
            watertankVisuals.transform.position = watertankPhysics.transform.position;

            Vector2 prevVel = velocity;
            velocity = (watertankPhysics.transform.position - prevPos) / Time.deltaTime;

            bool reverse = Vector2.Dot(velocity, prevVel) < 0;
            if(reverse) soundInstance.setParameterByName(GameManager.Instance.playerAudioData.tankMotion_IntTankMotionState, (int)(TankState.Reverse));
            //soundInstance.setVolume(Mathf.InverseLerp(0, maxSoundVelocity, velocity.magnitude));

            prevPos = watertankPhysics.transform.position;
        }
        else
        {
            soundInstance.setParameterByName(GameManager.Instance.playerAudioData.tankMotion_IntTankMotionState, ((int)TankState.Stop));
            velocity = Vector2.zero;
        }

        Debug.Log($"Velocity: {velocity} VelocityPHX: {watertankPhysics.linearVelocity}");
    }

    public float GetWaterLevelY()
    {
        return watertankPhysics.transform.position.y + waterLevel;
    }

    public void IsGrabed()
    {
        isGrabed = true; 
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z; // For 2D


        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        grabPoint = watertankPhysics.transform.InverseTransformPoint(mouseWorldPos);


        soundInstance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.tankMotion);
        soundInstance.setParameterByName(GameManager.Instance.playerAudioData.tankMotion_IntTankMotionState, ((int)TankState.Moving));
        soundInstance.start();
    }
    
    public void IsReleased()
    {
        soundInstance.setParameterByName(GameManager.Instance.playerAudioData.tankMotion_IntTankMotionState, ((int)TankState.Stop));
        isGrabed = false;
    }

    public void MoveGrabbed()
    {
        if (isGrabed)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = -Camera.main.transform.position.z;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector3 newWorldPos = mouseWorldPos - watertankPhysics.transform.TransformPoint(grabPoint);
            newWorldPos.y = startPoint.y; // Keep the water level Y
            newWorldPos.x = Mathf.Clamp(newWorldPos.x, startPoint.x - moveRange, startPoint.x + moveRange);
            targetPos = newWorldPos;


            return;

            Vector3 mouseDir = mouseWorldPos - new Vector3(watertankPhysics.transform.position.x, startPoint.y, mouseWorldPos.z);
            mouseDir.x = Mathf.Abs(mouseDir.x);
            float angle = Vector3.Angle(Vector3.right, mouseDir);
            angle = Mathf.Clamp(angle, 0, maxAngle);

            Vector3 pivot = lowestPoint;
            if (grabPoint.x > 0) lowestPoint.x = Mathf.Abs(lowestPoint.x);
            else lowestPoint.x = -Mathf.Abs(lowestPoint.x);

            pivot = watertankPhysics.transform.TransformPoint(pivot);

            // Move pivot to world space
            Vector3 dir = watertankPhysics.transform.position - pivot;

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

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0) return null;
        int randomIndex = Random.Range(1, spawnPoints.Length); // Start from 1 to skip the parent transform
        return spawnPoints[randomIndex];
    }

    public void DisableInteraction()
    {
        turnedOffInteractions = true;
        foreach (InteractableCollider2D i in interactables)
        {
            i.isReacting = false;
        }
    }

    public void EnableInteraction()
    {
        turnedOffInteractions = false;
        foreach (InteractableCollider2D i in interactables)
        {
            i.isReacting = true;
        }
    }
}
