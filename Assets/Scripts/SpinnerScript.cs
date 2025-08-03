using UnityEngine;

public class SpinnerScript : MonoBehaviour
{
    
    public float spinSpeed = 10f;
    public bool reverse = false;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (reverse) spinSpeed *= -1;
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }
}
