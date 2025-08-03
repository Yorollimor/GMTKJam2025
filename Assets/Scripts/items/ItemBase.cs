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

    private void Start()
    {
        FindAnyObjectByType<ScoreManager>().OnScoreChanged.AddListener(UpdateUI);
    }

    public void BuyItem()
    {
        GameManager.Instance.scoreManager.SpendPoints((int)price);
        price += priceIncrement;
        
        itemUI.price.text = price.ToString();
        Debug.Log($"[{name}] price changed to {price}");
    }

    public void UpdateUI(int score)
    {
        Color c = Color.white;
        if (score < price) c = Color.red;
        itemUI.price.color = c;
    }
}
