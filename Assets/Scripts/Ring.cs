using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public static Queue<FMOD.Studio.EventInstance> soundInstances = new Queue<FMOD.Studio.EventInstance>();

    public Color[] colors;
    public Vector2 minMaxImpactStrengths;
    Rigidbody2D rb;
    float outerRadius;
    float innerRadius;
    float area;
    float volume;

    private FMOD.Studio.EventInstance instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (colors.Length > 0) GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", colors[Random.Range(0, colors.Length)]);

        outerRadius = transform.localScale.y;
        innerRadius = outerRadius * 0.75f;
        area = Mathf.PI * (outerRadius * outerRadius - innerRadius * innerRadius);

        volume = (Mathf.PI * Mathf.PI / 4f) * (outerRadius + innerRadius) * Mathf.Pow(outerRadius - innerRadius, 2);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.transform.gameObject != this.gameObject)
        {
            float impactStrength = Mathf.InverseLerp(minMaxImpactStrengths.x, minMaxImpactStrengths.y, collision.relativeVelocity.magnitude);
            if (impactStrength > minMaxImpactStrengths.x)
            {
                EventReference impactSound = collision.gameObject.GetComponent<Ring>() ? GameManager.Instance.playerAudioData.loopsImpact : GameManager.Instance.playerAudioData.tankImpact;
                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(impactSound);
                if(Ring.soundInstances.Count >= 10)
                {
                    Ring.soundInstances.Dequeue().stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                }
                Debug.Log($"Impact on {gameObject.name} with strength {collision.relativeVelocity.magnitude}");
                instance.setParameterByName(GameManager.Instance.playerAudioData.loopsImpact_FloatImpactStrength, impactStrength);
                instance.start();
                Ring.soundInstances.Enqueue(instance);
            }
        }
    }

       
    private void FixedUpdate()
    {
        Vector2 force = GameManager.Instance.currentTank.GetTankVelocity();

        if (WaterStream.waterStreams.Count > 0)
        {
            //Water streams
            foreach (WaterStream es in WaterStream.waterStreams)
            {
                force += FindAnyObjectByType<WaterStream>().GetMoveVector(rb.position);
            }
        }

        
        //Add force
        if (force.magnitude > 0)
        {
            Debug.Log($"Force from Streams {gameObject.name}: {force}");
            rb.AddForce(force, ForceMode2D.Force);
        }
    }
}


/*OLD UPDATE
 * 
 *  Vector2 force = Vector2.zero;

        if (WaterStream.waterStreams.Count > 0)
        {
            //Water streams
            foreach (WaterStream es in WaterStream.waterStreams)
            {
                force += FindAnyObjectByType<WaterStream>().GetMoveVector(rb.position);
            }
            force /= WaterStream.waterStreams.Count;
        }

        //Drag force calculation
        Vector2 ringNormal = transform.up;
        Vector2 velocity = rb.linearVelocity;

        float alignment = Mathf.Abs(Vector3.Dot(ringNormal.normalized, velocity.normalized));
        float effectiveArea = area * alignment;

        force += -0.5f * physics.fluidDensity * velocity.normalized * velocity.sqrMagnitude * physics.dragCoefficientTorus * effectiveArea;

        //Gravity
        force += physics.gravity * rb.mass * Vector2.up;

        //Buoyancy force calculation
        float depth = GameManager.Instance.currentTank.GetWaterLevelY() - transform.position.y;
        if (depth > 0)
        {
            float submergedPortion = Mathf.Clamp01(depth / 1); //always assuming submerged
            float buoyancyForce = physics.fluidDensity * volume * submergedPortion * physics.gravity;

            Debug.Log($"Buoyancy on {gameObject.name}: {buoyancyForce}");

            //force += buoyancyForce * Vector2.up;
        }

        Debug.Log($"Force on {gameObject.name}: {force}");
        //Add force
        if (force.magnitude > 0)rb.AddForce(force, ForceMode2D.Force);
 * */