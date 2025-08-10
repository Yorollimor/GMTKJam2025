using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NonPlaceableArea : MonoBehaviour
{

    public int overlaps = 0;
    public Collider2D col;
    private List<Collider2D> overlappingColliders = new List<Collider2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        overlappingColliders.Add(collision);
        overlaps++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlappingColliders.Remove(collision);
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

    public Collider2D GetFirstOverlap()
    {
        if (IsOverlapping()) return overlappingColliders[0];
        else return null;

    }
    public Collider2D GetLastOverlap()
    {
        if (IsOverlapping()) return overlappingColliders[overlappingColliders.Count - 1];
        else return null;
    }
    public bool IsOverlapping()
    {
        return overlaps > 0;
    }
}
