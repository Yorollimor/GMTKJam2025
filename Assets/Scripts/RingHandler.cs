using UnityEngine;

public class RingHandler : MonoBehaviour
{
    public bool isHooked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hook"))
        {
            OnHooked();
        }
    }

    public void OnHooked()
    {
        if (isHooked) return;

        isHooked = true;
        // Optional: Add visual feedback for being hooked

        // Notify the manager
        FindObjectOfType<RingManager>().OnRingHooked(gameObject);
    }
}