using System.Collections;
using UnityEngine;

public class RingHandler : MonoBehaviour
{
    public bool isHooked = false;
    public float stackLifetime = 2f; // Set in Inspector (2 or 5 seconds)

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    public void OnHooked()
    {
        if (isHooked) return;
        isHooked = true;

        // Freeze the ring in place (no snapping)
        /*
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        */

        // Parent to Hook (optional) — or keep world position
        // transform.SetParent(null);

        ComboManager.Instance.AddScore(10);
        Debug.Log("Score Added!");

        // Start destruction countdown
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(stackLifetime);
        Destroy(gameObject);
    }

}