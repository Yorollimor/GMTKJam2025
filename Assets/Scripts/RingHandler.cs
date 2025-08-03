using System.Collections;
using UnityEngine;

public class RingHandler : MonoBehaviour
{
    public bool isHooked = false;
    HookTrigger hookedBy;

    public float defaultDestroyDelay = 2f; // Set in Inspector (2 or 5 seconds)

    private float destroyDelay;

    public int basePoints = 10; // Base points for the ring, can be set in Inspector
    private int comboMultiplier = 1;

    private Rigidbody2D rb;
    private MeshRenderer meshRenderer;

    float popTimer = -1, popDuration;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Update()
    {
        if(popTimer >= 0)
        {
            popTimer -= Time.deltaTime;
            meshRenderer.material.SetFloat("_pop", 1-(popTimer/ popDuration));
            if (popTimer <= 0)
            {
                meshRenderer.material.SetFloat("_pop", 1);
                popTimer = -1;
            }
        }
    }

    public void OnHooked(HookTrigger hook)
    {
        if (isHooked || !hook) return;
        hook.comboCounter.AddToCombo(this);
        hookedBy = hook;
        isHooked = true;

        // Freeze the ring in place (no snapping)
        /*
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        */

        // Parent to Hook (optional) — or keep world position
        // transform.SetParent(null);s
    }

    public void OnUnHooked()
    {
        if (hookedBy) hookedBy.comboCounter.RemoveFromCombo(this);
        hookedBy = null;
        isHooked = false;
    }

    public void StartCounting(int combo = 1, float destroyDelay = -1.0f)
    {

        this.destroyDelay = destroyDelay == -1.0f ? defaultDestroyDelay : destroyDelay;
        comboMultiplier = combo;
        StartCoroutine(DestroyAfterDelay());
    }
    private IEnumerator DestroyAfterDelay()
    {

        yield return new WaitForSeconds(destroyDelay);

        TriggerPopAnimation(0.5f);


        yield return new WaitForSeconds(0.5f);

        FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.loopsVanish);
        instance.start();

        GameManager.Instance.ringManager.RingDestroyed(gameObject);
        GameManager.Instance.scoreManager.UpdateScore(basePoints * comboMultiplier);
        Destroy(gameObject);
    }

    public void TriggerPopAnimation(float diration)
    {
        popTimer = popDuration = diration;
    }
}