using UnityEngine;

public class NonPlaceableArea : MonoBehaviour
{

    public int overlaps = 0;
   public Collider2D col;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        overlaps++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlaps--;
    }

    /// <summary>
    /// Called when the item is being dragged from the shop and sets the collider of this component to trigger
    /// </summary>
    public void DraggedItem()
    {
        col.isTrigger = true;
    }
    /// <summary>
    /// sets the collider back to non-trigger
    /// </summary>
    public void DraggedPlaced()
    {
        col.isTrigger = false;
    }
}
