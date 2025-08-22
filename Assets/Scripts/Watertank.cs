
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

    /// <summary>
    /// Must be in this order: Top Left, Top Right, Bottom Left, Bottom Right. 
    /// </summary>
    public Transform[] placementBoundaries;

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
    public GameObject scoreSpawnOBJ;
    private Transform[] spawnPoints;

    public TextMeshPro scoreText;
    public Transform moveableObjectsParent;

    Vector2 velocity;
    Vector3 prevPos;
    public float velocityDragMultiplier = 0.95f;

    public InteractableCollider2D[] interactables;
    public InteractableCollider2D storeButton;

    FMOD.Studio.EventInstance soundInstance;
    TankState soundState = TankState.Stop;
    public float maxSoundVelocity;

    private bool turnedOffInteractions = false;
    private int altIndex = 1;
    private void Awake()
    {

        spawnPoints = spawnPointParent.GetComponentsInChildren<Transform>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPoint = targetPos = watertankPhysics.transform.position;
        targetRot = watertankPhysics.transform.rotation.eulerAngles.z;
        GameManager.Instance.currentTank = this;


        soundInstance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.tankMotion);
        soundInstance.start();

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
            //soundInstance.setVolume();

            prevPos = watertankPhysics.transform.position;
        }
        else
        {
            soundInstance.setParameterByName(GameManager.Instance.playerAudioData.tankMotion_IntTankMotionState, ((int)TankState.Stop));
            velocity = Vector2.zero;
        }

        soundInstance.setParameterByName(GameManager.Instance.playerAudioData.tankMotion_FloatTankVelocity, Mathf.InverseLerp(0, maxSoundVelocity, velocity.magnitude));

        //Debug.Log($"Velocity: {velocity} VelocityPHX: {watertankPhysics.linearVelocity}");
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

        soundInstance.setParameterByName(GameManager.Instance.playerAudioData.tankMotion_IntTankMotionState, ((int)TankState.Moving));

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

    public Transform GetSpawnPoint()
    {
        if (spawnPoints.Length == 0) return null;
        if (altIndex >= spawnPoints.Length)
            altIndex = 1; // Start from 1 to skip the parent transform
        return spawnPoints[altIndex++];
    }

    public void DisableInteraction()
    {
        turnedOffInteractions = true;
        foreach (InteractableCollider2D i in interactables)
        {
            i.isReacting = false;
        }
        storeButton.isReacting = false;
    }

    public void EnableInteraction()
    {
        turnedOffInteractions = false;
        foreach (InteractableCollider2D i in interactables)
        {
            i.isReacting = true;
        }
        storeButton.isReacting = true;
    }

    public void SwapTanks(Watertank newTank)
    {
        foreach (Ring r in FindObjectsByType<Ring>(FindObjectsSortMode.None))
        {
            Destroy(r.gameObject);
            FindFirstObjectByType<RingManager>().RingDestroyed(r.gameObject);
        }
        foreach (Transform child in moveableObjectsParent.GetComponentsInChildren<Transform>())
        {
            if (child.parent != moveableObjectsParent) continue;
            Vector3 localPos = child.localPosition;
            Quaternion localRot = child.localRotation;


            child.SetParent(newTank.moveableObjectsParent, false); // 'false' keeps local position
            child.localPosition = localPos;
            child.localRotation = localRot;
        }
        GameManager.Instance.currentTank = newTank;
        Destroy(gameObject); // Destroy the old tank instance
    }

    public Transform[] GetPlacementBoundaries()
    {
        return placementBoundaries;
    }

    public GameObject GetScoreSpawnLocation()
    {
        return scoreSpawnOBJ;
    }        
}
