using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

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
    private float _settingsPos;
    
    private bool _isShopOpen;
    private bool _isSettingsOpen;
    
    public ScoreManager scoreManager;

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

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager.OnScoreChanged.AddListener(UpdateShopScore);
        raycastBlocker.gameObject.SetActive(false);
        
        _shopPos = panelShop.position.x;
        _settingsPos = panelSettings.position.x;
        
        panelShop.DOMoveX(_shopPos + Screen.width, 0f, true);
        _isShopOpen = false;
        _isSettingsOpen = true;
        Camera.main.transform.DOMoveX(gamePositionCenter - gamePositionDelta, 0, true);
        ToggleBlocker();
        //panelSettings.DOMoveX(_settingsPos - Screen.width, 0f, true);
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
            panelShop.DOMoveX(_shopPos, animSpeed, false).SetEase(Ease.OutSine);
            MoveGame(GameAlignment.Left);
        }
        else
        {
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
            panelSettings.DOMoveX(_settingsPos, animSpeed, false).SetEase(Ease.OutSine);
            MoveGame(GameAlignment.Right);
        }
        else
        {
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

    private void MoveGame(GameAlignment alignment)
    {
        switch (alignment)
        {
            case GameAlignment.Right:
                Camera.main.transform.DOMoveX(gamePositionCenter - gamePositionDelta, animSpeed, false).SetEase(Ease.OutSine);
                break;
            case GameAlignment.Center:
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
}
