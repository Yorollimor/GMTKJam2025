using UnityEngine;

public class HookTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform ringParent = collision.transform.parent;
        if (ringParent != null && ringParent.CompareTag("Ring"))
        {
            RingHandler ringScript = ringParent.GetComponent<RingHandler>();
            if (ringScript != null && !ringScript.isHooked)
            {
                ringScript.OnHooked();
            }
        }
    }
}