using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class Spurt : MonoBehaviour
{
    public float spurtStrength = 1.0f;
    public Vector2 minMaxSpurtDist = new Vector2(0,10);
    public float pullStrength = 1.0f; // Strength of the pull when rings are flipped
    public Vector2 minMaxPullDist = new Vector2(0, 10);

    public float pullToHeight = 2.0f;
    public float alwaysPushRadius = 2.0f;
    public float maxTourque = 0.001f;

    public float flipAngle = 30.0f; // Angle where rings get pulled instead of pushed

    public float streamLength, streamStrength, streamSpeed;

    List<RingHandler> rings = new List<RingHandler>();


    private void Start()
    {

        Debug.Log($"spurtStrength: {spurtStrength}, minMaxSpurtDist: {minMaxSpurtDist} pullStrength: {pullStrength} minMaxPullDist: {minMaxPullDist} me: {gameObject.name} streamLength: {streamLength} streamStrength: {streamStrength}");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerSpurt();
        }
    }

    void UpdateRingsList()
    {
        rings.Clear();
        rings.AddRange(FindObjectsByType<RingHandler>(FindObjectsSortMode.None));
    }

    public void TriggerSpurt()
    {
        UpdateRingsList();


        FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.water_whoosh);
        instance.start();

        foreach (WaterStream es in WaterStream.waterStreams)
        {
            es.StartStream(streamLength, streamStrength, streamSpeed);
        }

        foreach (RingHandler ring in rings)
        {
            if (ring != null)
            {
                Vector2 force = GetVelocityAtPosition(ring.transform.position);
                ring.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
                float torque = GetTorqueAtPosition(ring.transform.position, ring.transform.rotation);
                ring.GetComponent<Rigidbody2D>().AddTorque(torque, ForceMode2D.Impulse);
            }
        }
    }

    Vector2 GetVelocityAtPosition(Vector3 pos)
    {

        Vector3 direction = pos - transform.position;
        float distance = direction.magnitude;

        // Early exit if out of all ranges
        if (distance > Mathf.Max(minMaxPullDist.y, minMaxSpurtDist.y))
            return Vector2.zero;

        float angle = Vector2.Angle(Vector2.up, direction);
        Vector2 force = Vector2.zero;
        float strength = 0f;

        if (angle > flipAngle && distance > alwaysPushRadius)
        {
            float distFactor = 1 - Mathf.InverseLerp(minMaxPullDist.x, minMaxPullDist.y, distance);
            float angleFactor = Mathf.InverseLerp(flipAngle, 90, angle);
            strength = -pullStrength * distFactor * angleFactor;

            force = direction.normalized * strength;
        }
        else
        {
            float distFactor = 1 -Mathf.InverseLerp(minMaxSpurtDist.x, minMaxSpurtDist.y, distance);
            float angleFactor = Mathf.InverseLerp(0, flipAngle, angle);
            strength = spurtStrength * distFactor * angleFactor;

            strength *= Random.Range(0.8f, 1.0f); // Randomize force a bit for variety

            direction = pos - (transform.position + Vector3.up * pullToHeight);

            force = Vector2.Lerp(direction.normalized, Vector2.up, Random.Range(0.35f, 0.7f)) * strength;
        }

        Debug.Log($"Force: {force}, Strength: {strength}, Angle: {angle}, Distance: {distance}");
        return force;
    }

    float GetTorqueAtPosition(Vector3 pos, Quaternion rotation)
    {
        float rotZDeg = rotation.eulerAngles.z % 180;
        Vector3 direction = pos - transform.position;
        bool isToTheRight = direction.x > 0;
        bool isRightSideUp = rotZDeg % 180 <= 90;

        float strength = 1 - Mathf.InverseLerp(minMaxSpurtDist.x, minMaxSpurtDist.y, direction.magnitude);
        if(isRightSideUp) strength *= (1 - Mathf.InverseLerp(0, 90, rotZDeg % 180));
        else strength *= Mathf.InverseLerp(90, 180, rotZDeg % 180);

        float torque = (isToTheRight && isRightSideUp) || (!isToTheRight && !isRightSideUp) ? 1 : -1;
        torque *= Random.Range(0.5f, 1.0f); // Randomize torque a bit for variety
        torque = torque * strength * maxTourque;
        Debug.Log($"Torque: {torque}, Strength: {strength} spurtTorque: {maxTourque}");
        return torque;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * pullToHeight);
        Gizmos.DrawWireSphere(transform.position, alwaysPushRadius);
    }
}
