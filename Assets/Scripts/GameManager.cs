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
        };
        SceneManager.sceneLoaded += InitializeManager;

        InitializeManager(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void InitializeManager(Scene scene, LoadSceneMode mode)
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        ringManager = FindFirstObjectByType<RingManager>();
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        currentTank = FindFirstObjectByType<Watertank>();

        if(FindObjectsByType<ScoreManager>(FindObjectsSortMode.None).Length > 1) Debug.LogError("Multiple ScoreManager instances found! This should not happen. Please check your scene setup.");
        if(FindObjectsByType<RingManager>(FindObjectsSortMode.None).Length > 1) Debug.LogError("Multiple RingManager instances found! This should not happen. Please check your scene setup.");
        if(FindObjectsByType<UpgradeManager>(FindObjectsSortMode.None).Length > 1) Debug.LogError("Multiple UpgradeManager instances found! This should not happen. Please check your scene setup.");
        if(FindObjectsByType<Watertank>(FindObjectsSortMode.None).Length > 1) Debug.LogError("Multiple Watertank instances found! This should not happen. Please check your scene setup.");

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
