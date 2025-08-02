using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private enum GameAlignment { Left,Center,Right }
    
    public float animSpeed = 1.0f;
    public float gamePositionCenter = 0.0f;
    public float gamePositionDelta = 50.0f;
    
    public Button buttonShop;
    public Button buttonSettings;
    public Button raycastBlocker;

    public Transform panelGame;
    public RectTransform panelShop;
    public RectTransform panelSettings;

    private float _shopPos;
    private float _settingsPos;
    
    private bool _isShopOpen = true;
    private bool  _isSettingsOpen = true;
    
    private void OnEnable()
    {
        buttonShop.onClick.AddListener(ToggleShop);
        buttonSettings.onClick.AddListener(ToggleSettings);
        raycastBlocker.onClick.AddListener(CloseMenu);
    }


    private void OnDisable()
    {
        buttonShop.onClick.RemoveListener(ToggleShop);
        buttonSettings.onClick.RemoveListener(ToggleSettings);
        raycastBlocker.onClick.RemoveListener(CloseMenu);
    }

    private void Start()
    {
        panelGame.position = new Vector3(gamePositionDelta, 0f, 0f);
        
        raycastBlocker.gameObject.SetActive(false);

        _shopPos = panelShop.position.x;
        _settingsPos = panelSettings.position.x;

        ToggleShop();
    }

    private void CloseMenu()
    {
        Debug.Log("CloseMenu");
        if (_isShopOpen)
            ToggleShop();
        
        if (_isSettingsOpen)
            ToggleSettings();
    }
    
    private void ToggleShop()
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

    private void ToggleSettings()
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
    }

    private void MoveGame(GameAlignment alignment)
    {
        switch (alignment)
        {
            case GameAlignment.Left:
                panelGame.DOMoveX(gamePositionCenter - gamePositionDelta, animSpeed, false).SetEase(Ease.OutSine);
                break;
            case GameAlignment.Center:
                panelGame.DOMoveX(gamePositionCenter, animSpeed, false).SetEase(Ease.OutSine);
                break;
            case GameAlignment.Right:
                panelGame.DOMoveX(gamePositionCenter + gamePositionDelta, animSpeed, false).SetEase(Ease.OutSine);
                break;
        }
    }
}
