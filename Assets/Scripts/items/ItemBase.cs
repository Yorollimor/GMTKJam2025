using System;
using UnityEngine;

[RequireComponent(typeof(ItemUI))]
public class ItemBase : MonoBehaviour
{
    public ItemType itemType;
    public ItemUI itemUI;
    
    public new string name = "";
    public string description = "";
    public float price;
    private float basePrice; //startValue
    public float priceIncrement;
    public Sprite sprite;

    protected int maxBuyCount = -1; // Maximum number of times this item can be bought -1 = unlimited
    private int buyCount = 0; // Current number of times this item has been bought

    private void Awake()
    {
        basePrice = price;
        itemUI.image.sprite = sprite;
        itemUI.name.text = name;
        itemUI.price.text = price.ToString();
        itemUI.description.text = description;
    }

    protected virtual void Start()
    {
        GameManager.Instance.scoreManager.OnScoreChanged.AddListener(UpdateUI);
        UpdateUI(0);
    }

    public void BuyItem()
    {
        GameManager.Instance.scoreManager.SpendPoints((int)price);

        buyCount++;

        price += priceIncrement; 

        Debug.Log($"[{name}] price changed to {price}");
        UpdateUI(GameManager.Instance.scoreManager.GetScore());

    }

    public void DestroyedItem()
    {
        //Refund?
        //GameManager.Instance.scoreManager.UpdateScore((int)price);

        buyCount--;
        price = Mathf.Max(basePrice, price - priceIncrement);

        UpdateUI(GameManager.Instance.scoreManager.GetScore());
    }

    public void UpdateUI(int score)
    {
        if (IsSoldOut())
        {
            itemUI.price.text = "SOLD OUT";
            itemUI.currencyIcon.gameObject.SetActive(false); // Hide the currency icon
            itemUI.image.color = Color.gray; // Disable the item visually
            Debug.Log($"[{name}] has reached its maximum buy count of {maxBuyCount} and is now disabled.");
        }
        else
        {
            itemUI.price.text = price.ToString();
            Color c = Color.white;
            if (score < price) c = new Color(0.8f,0,0);
            itemUI.price.color = c;
            itemUI.currencyIcon.color = c;
        }
    }

    public bool IsSoldOut()
    {
        return maxBuyCount > 0 && buyCount >= maxBuyCount;
    }
}
