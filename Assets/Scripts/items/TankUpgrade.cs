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

    private void Start()
    {
        maxBuyCount = GameManager.Instance.upgradeManager.GetMaxLevel(itemType);
    }

    private void BuyUpgrade()
    {
        //TODO: Add validation
        bool canAfford = GameManager.Instance.scoreManager.GetScore() >= (int)(price);

        FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(canAfford ? GameManager.Instance.playerAudioData.upgradeBuy : GameManager.Instance.playerAudioData.upgradeFail);
        instance.start();

        if (!canAfford) return;
        GameManager.Instance.upgradeManager.UpgradeItem(itemType);
        base.BuyItem();

        //TODO: get tank reference and apply the type of upgrade
    }
}
