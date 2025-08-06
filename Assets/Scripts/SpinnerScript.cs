using UnityEngine;

public class SpinnerScript : MonoBehaviour
{
    
    public float spinSpeed = 10f;
    public bool reverse = false;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (reverse) spinSpeed *= -1;
        sr.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }
}
