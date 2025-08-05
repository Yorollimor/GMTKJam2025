using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerAudioData playerAudioData;
    public Watertank currentTank;
    public ScoreManager scoreManager;
    public RingManager ringManager;
    public TransitionManager transitionManager;
    public UpgradeManager upgradeManager;

    public int buildIndex_startScene = 0;
    public int buildIndex_mainMenu = 2;
    public int buildIndex_mainScene = 1;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(this); // Destroy duplicate instances
        }
    }


    public void LoadMainScene()
    {
        SceneManager.LoadScene(buildIndex_mainScene);
    }
    public void LoadStartScene()
    {
        SceneManager.LoadScene(buildIndex_startScene);
    }
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(buildIndex_mainMenu);
    }

}
