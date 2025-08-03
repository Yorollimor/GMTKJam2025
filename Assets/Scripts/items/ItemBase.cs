using System;
using UnityEngine;

[RequireComponent(typeof(ItemUI))]
public class ItemBase : MonoBehaviour
{
    public ItemUI itemUI;
    
    public new string name = "";
    public float price;
    public float priceIncrement;
    public Sprite sprite;
    
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
