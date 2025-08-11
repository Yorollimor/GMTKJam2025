using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public enum UpgradeVariant
{
    Base,
    Draggable,
    Upgrade,
}

public class UIManager : MonoBehaviour
{
    private enum GameAlignment { Left,Center,Right }
    
    public float animSpeed = 1.0f;
    public float gamePositionCenter = 0.0f;
    public float gamePositionDelta = 50.0f;
    
    public InteractableCollider2D buttonShop;
    public Button buttonSettings;
    public Button buttonStart;
    public Button raycastBlocker;

    public RectTransform panelShop;
    public RectTransform panelSettings;
    public TextMeshProUGUI shopScore;
    
    private float _shopPos;
    private bool _isShopOpen;
    
    private float _settingsPos;
    private bool _isSettingsOpen;
    
    public ScoreManager scoreManager;

    public ItemBase[] shopItems;
    public DraggableUpgrade[] shopDraggables;
    public TankUpgrade[] shopUpgrades;

    private void Awake()
    {
        shopItems = GetComponentsInChildren<ItemBase>();
        shopDraggables = GetComponentsInChildren<DraggableUpgrade>();
        shopUpgrades = GetComponentsInChildren<TankUpgrade>();
    }

    private void OnEnable()
    {
        buttonShop.OnReleased.AddListener(ToggleShop);
        buttonSettings.onClick.AddListener(ToggleSettings);
        buttonStart.onClick.AddListener(ToggleSettings);
        raycastBlocker.onClick.AddListener(CloseMenu);
    }


    private void OnDisable()
    {
        buttonShop.OnReleased.RemoveListener(ToggleShop);
        buttonSettings.onClick.RemoveListener(ToggleSettings);
        buttonStart.onClick.RemoveListener(ToggleSettings);
        raycastBlocker.onClick.RemoveListener(CloseMenu);
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdatePanelPositions();
    }
    
    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager.OnScoreChanged.AddListener(UpdateShopScore);
        raycastBlocker.gameObject.SetActive(false);

        UpdatePanelPositions();
        
        panelShop.DOMoveX(_shopPos + Screen.width, 0f, true);
        _isShopOpen = false;
        _isSettingsOpen = true;
        Camera.main.transform.DOMoveX(gamePositionCenter - gamePositionDelta, 0, true);
        ToggleBlocker();
        shopScore.text = "0";
        //panelSettings.DOMoveX(_settingsPos - Screen.width, 0f, true);
    }
    
    
    private void UpdatePanelPositions()
    {
        _shopPos = Screen.width / 2;
        _settingsPos = _shopPos;
    }
    
    public void CloseMenu()
    {
        Debug.Log("CloseMenu");
        if (_isShopOpen)
            ToggleShop();
        
        if (_isSettingsOpen)
            ToggleSettings();
    }
    
    public void ToggleShop()
    {
        if (!_isShopOpen)
        {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.menu_upgradeSlideOut);
            instance.start();

            panelShop.DOMoveX(_shopPos, animSpeed, false).SetEase(Ease.OutSine);
            MoveGame(GameAlignment.Left);
        }
        else
        {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.menu_upgradeSlideIn);
            instance.start();
            panelShop.DOMoveX(_shopPos + Screen.width, animSpeed, false).SetEase(Ease.InSine);
            MoveGame(GameAlignment.Center);
        }
        
        ToggleBlocker();
        _isShopOpen = !_isShopOpen;
    }

    public void ToggleSettings()
    {
        if (!_isSettingsOpen)
        {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.menu_upgradeSlideOut);
            instance.start();

            panelSettings.DOMoveX(_settingsPos, animSpeed, false).SetEase(Ease.OutSine);
            MoveGame(GameAlignment.Right);
        }
        else
        {

            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.menu_upgradeSlideIn);
            instance.start();
            panelSettings.DOMoveX(_settingsPos - Screen.width, animSpeed, false).SetEase(Ease.InSine);
            MoveGame(GameAlignment.Center);
        }
        
        ToggleBlocker();
        _isSettingsOpen = !_isSettingsOpen;
    }

    private void ToggleBlocker()
    {
        bool isEnabled = raycastBlocker.gameObject.activeSelf;
        raycastBlocker.gameObject.SetActive(!isEnabled);

        if(!isEnabled) FindAnyObjectByType<Watertank>().DisableInteraction();
        else FindAnyObjectByType<Watertank>().EnableInteraction();
    }

    public void SetBlockerActive(bool isActive)
    {
        raycastBlocker.gameObject.SetActive(isActive);
    }

    private void MoveGame(GameAlignment alignment)
    {
        gamePositionCenter = GameManager.Instance.currentTank.watertankVisuals.transform.position.x;
        switch (alignment)
        {
            case GameAlignment.Right:
                Camera.main.transform.DOMoveX(gamePositionCenter - gamePositionDelta, animSpeed, false).SetEase(Ease.OutSine);
                break;
            case GameAlignment.Center:
                gamePositionCenter = 0;
                Camera.main.transform.DOMoveX(gamePositionCenter, animSpeed, false).SetEase(Ease.OutSine);
                break;
            case GameAlignment.Left:
                Camera.main.transform.DOMoveX(gamePositionCenter + gamePositionDelta, animSpeed, false).SetEase(Ease.OutSine);
                break;
        }
    }

    private void UpdateShopScore(int newScore)
    {
        shopScore.text = newScore.ToString();

    }

    public void SetNewStoreButton(InteractableCollider2D newB)
    {
        buttonShop.OnReleased.RemoveListener(ToggleShop);
        buttonShop = newB;
        buttonShop.OnReleased.AddListener(ToggleShop);
    }

    public ItemBase GetShopItem(ItemType itemType)
    {
        return GetShopItem<ItemBase>(itemType);
    }
    public T GetShopItem<T>(ItemType itemType) where T : ItemBase
    {
        ItemBase[] items = null;

        if (typeof(T) == typeof(ItemBase))
            items = shopItems;
        else if (typeof(T) == typeof(DraggableUpgrade))
            items = shopDraggables;
        else if (typeof(T) == typeof(TankUpgrade))
            items = shopUpgrades;

        if (items != null)
        {
            foreach (var item in items)
            {
                if (item.itemType == itemType)
                    return item as T;
            }
        }

        return null;
    }
}
