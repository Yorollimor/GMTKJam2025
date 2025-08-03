using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TankUpgrade : ItemBase
{
    private Button _button;
    private void OnEnable()
    {
        if (base.itemUI.image.GetComponent<Button>() == null)
            _button = base.itemUI.image.AddComponent<Button>();
        
        _button.onClick.AddListener(BuyUpgrade);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(BuyUpgrade);
    }
    
    private void BuyUpgrade()
    {
        base.BuyItem();
        
        //TODO: get tank reference and apply the type of upgrade
    }
}
