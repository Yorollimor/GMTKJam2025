using UnityEngine;

public class BumperScript : MonoBehaviour
{
    public float force = 2;
    public float torque = 0.5f;

   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ring"))
        {
            print("ring hit");
            
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            Vector3 center = gameObject.transform.position;
            Vector3 direction = rb.gameObject.transform.position - center;
            direction = Vector3.Normalize(direction);

            rb.AddForce(direction * force);
            rb.AddTorque(torque);
        }
    }
}
