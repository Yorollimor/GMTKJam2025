using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacableItem : MonoBehaviour
{
    public ItemType itemType;
    public int upgradeLevel;

    public SpriteRenderer[] spriteRenderers;
    public MeshRenderer[] meshRenderers;

    public NonPlaceableArea nonPlaceableArea;
    public Collider2D[] colliders;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        nonPlaceableArea = GetComponentInChildren<NonPlaceableArea>();

        List<Collider2D> allColliders = new List<Collider2D>();
        allColliders.AddRange(GetComponentsInChildren<Collider2D>());
        if(nonPlaceableArea) allColliders.Remove(nonPlaceableArea.col);
        colliders = allColliders.ToArray();
    }

    private void OnDestroy()
    {
        DraggableUpgrade upgrade = GameManager.Instance.UIManager.GetShopItem<DraggableUpgrade>(itemType);
        if (upgrade)
        {
            upgrade.DestroyedItem();
        }
    }

    public void SetColor(Color c)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = c;
        }
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = c;
        }
    }

    public void SetVisibility(bool isVisible)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = isVisible;
        }
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = isVisible;
        }
    }

    public void SetPhysicsInteractions(bool hasPhysicsInteractions)
    {
        foreach (Collider2D col in colliders)
        {
            col.enabled = hasPhysicsInteractions;
        }
    }

    public void SetNonPlacableAreaActive(bool isActive)
    {
        nonPlaceableArea.col.enabled = isActive;
    }
}