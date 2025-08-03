using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    private Vector3 cameraTargetPos;

    float transitionTimer = -1;
    public float transitionDuration;
    public float offset = 10;

    public Animator mainMenu, shopMenu;

    bool menuOpen = false;
    bool shopOpen = false;

    public bool openMenuOnStart = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.transitionManager = this;
    }

    // Update is called once per frame
    void Update()
    {

        if (openMenuOnStart)
        {
            openMenuOnStart = false;
            Camera.main.transform.position = new Vector3(-offset, Camera.main.transform.position.y, Camera.main.transform.position.z);
            OpenMenu();
        }

        if (transitionTimer >= 0)
        {
            transitionTimer -= Time.deltaTime;
            Debug.Log("Transitioning: " + cameraTargetPos+ " ratio "+ Mathf.Pow(1 - (transitionTimer / transitionDuration), 2));
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTargetPos,Mathf.Pow(1 - (transitionTimer / transitionDuration), 2));
            if (transitionTimer <= 0)
            {
                Camera.main.transform.position = cameraTargetPos;
                transitionTimer = -1;
            }
        }
        
    }

    public void OpenMenu()
    {
        if (menuOpen) return;
        mainMenu.SetBool("MenuOpen", true);
        menuOpen = true;

        GameManager.Instance.currentTank.DisableInteraction();
        transitionTimer = transitionDuration;
        cameraTargetPos = Camera.main.transform.position;
        cameraTargetPos.x = GameManager.Instance.currentTank.transform.position.x - offset;
    }

    public void OpenShop()
    {
        if (shopOpen) return;
        shopMenu.SetBool("ShopOpen", true);
        shopOpen = true;

        GameManager.Instance.currentTank.DisableInteraction();
        transitionTimer = transitionDuration;
        cameraTargetPos = Camera.main.transform.position;
        cameraTargetPos.x = GameManager.Instance.currentTank.transform.position.x + offset;
    }

    public void BackToGame()
    {
        if (menuOpen)
        {
            mainMenu.SetBool("MenuOpen", false);
            menuOpen = false;
        }
        else if (shopOpen)
        {
            shopMenu.SetBool("ShopOpen", false);
            shopOpen = false;
        }
        else return;

        GameManager.Instance.currentTank.EnableInteraction();
        transitionTimer = transitionDuration;
        cameraTargetPos = Camera.main.transform.position;
        cameraTargetPos.x = 0;
    }

    public void ToggleMenu()
    {
        if (menuOpen)
        {
            BackToGame();
        }
        else
        {
            OpenMenu();
        }
    }

}
