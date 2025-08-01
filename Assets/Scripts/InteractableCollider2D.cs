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

    bool isPressed;

    Collider2D collider2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<Collider2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseEnter()
    {
        OnHovered.Invoke();
        //Debug.Log("Mouse hover");
    }


    void OnMouseExit()
    {
        OnUnhovered.Invoke();
        //Debug.Log("Mouse unhover");
    }
    void OnMouseOver()
    {
        WhileHovered.Invoke();
        if(isPressed) WhilePressed.Invoke();    
        //Debug.Log("Mouse over");
    }

    void OnMouseDown()
    {

        OnClicked.Invoke();
        isPressed = true;
        //Debug.Log("Mouse clicked");
    }
    void OnMouseUp()
    {

        OnReleased.Invoke();
        isPressed = false;
        //Debug.Log("Mouse released");
    }
}
