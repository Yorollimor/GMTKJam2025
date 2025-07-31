using UnityEngine;

public class HookTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ring"))
        {
            RingHandler ringScript = collision.GetComponent<RingHandler>();
            if (ringScript != null && !ringScript.isHooked)
            {
                ringScript.OnHooked();
            }
        }
    }
}