using UnityEngine;
using UnityEngine.Events;

public class InteractableCollider2D : MonoBehaviour
{
    public UnityEvent OnHovered;
    public UnityEvent OnUnhovered;
    public UnityEvent OnClicked;
    public UnityEvent OnReleased;
    public UnityEvent WhilePressed;
    public UnityEvent WhileHovered;

    bool isPressed, isHovering;
    public bool isReacting = true;

    public KeyCode triggerByKey = KeyCode.None; // Default to left mouse button

    Collider2D collider2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<Collider2D>();   
    }

    private void Update()
    {
        if (!isReacting) return;
    

        if (triggerByKey != KeyCode.None)
        {
            if (Input.GetKeyDown(triggerByKey))
            {
                OnClicked.Invoke();
                isPressed = true;
            }
            else if (Input.GetKeyUp(triggerByKey))
            {
                OnReleased.Invoke();
                isPressed = false;
            }
        } 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isReacting) return;
        if (isPressed) WhilePressed.Invoke();
        if(isHovering) WhileHovered.Invoke();


    }
    void OnMouseEnter()
    {
        if (!isReacting) return;
        OnHovered.Invoke();
        isHovering = true;
        //Debug.Log("Mouse hover");
    }


    void OnMouseExit()
    {
        if (!isReacting) return;
        OnUnhovered.Invoke();
        isHovering = false;
        //Debug.Log("Mouse unhover");
    }
    void OnMouseDown()
    {

        if (!isReacting) return;
        OnClicked.Invoke();
        isPressed = true;
        //Debug.Log("Mouse clicked");
    }
    void OnMouseUp()
    {

        if (!isReacting) return;
        OnReleased.Invoke();
        isPressed = false;
        //Debug.Log("Mouse released");
    }

    public void TriggerPress()
    {
        OnMouseDown();
    }

    public void TriggerRelease()
    {
        OnMouseUp();
    }
}
