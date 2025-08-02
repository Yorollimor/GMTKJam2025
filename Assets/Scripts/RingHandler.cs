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

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    private void Update()
    {
        if (isHooked && hookedBy)
        {
            if (!hookedBy.isOnHook(gameObject))
            {
                OnUnHooked();
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

    private void OnUnHooked()
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
        GameManager.Instance.scoreManager.UpdateScore(basePoints * comboMultiplier);
        Destroy(gameObject);
    }

}