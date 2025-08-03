using System;
using UnityEngine;

[RequireComponent(typeof(ItemUI))]
public class ItemBase : MonoBehaviour
{
    public new string name = "";
    public float price;
    public float priceIncrement;
    public Sprite sprite;
    public GameObject itemPrefab;
    
    public ItemUI itemUI;

    private void Awake()
    {
        itemUI.image.sprite = sprite;
        itemUI.name.text = name;
        itemUI.price.text = price.ToString();
    }

    public void BuyItem()
    {
        price += priceIncrement;
        Debug.Log($"[{name}] price changed to {price}");
    }
}
