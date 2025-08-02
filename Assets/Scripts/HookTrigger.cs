using Unity.VisualScripting;
using UnityEngine;

public class HookTrigger : MonoBehaviour
{

    public ComboCounter comboCounter;
    public float hookBaseWidth = 0.5f; // Base width of the hook trigger
    public bool isOnHook(GameObject go)
    {
        return go.transform.position.y > transform.position.y && go.transform.position.x > transform.position.x - hookBaseWidth / 2f && go.transform.position.x < transform.position.x + hookBaseWidth / 2f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Hook Trigger Entered: {collision.gameObject.name}");
        Transform ringParent = collision.transform.parent;
        if (ringParent != null && ringParent.CompareTag("Ring"))
        {
            Debug.Log($"Hook Trigger Entered:is Ring");
            RingHandler ringScript = ringParent.GetComponent<RingHandler>();
            if (ringScript != null && !ringScript.isHooked && isOnHook(ringScript.gameObject))
            {
                ringScript.OnHooked(this);
                Debug.Log("Score Added!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, hookBaseWidth/2.0f); // Visualize the hook area
    }
}