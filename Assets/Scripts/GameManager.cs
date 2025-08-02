using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerAudioData playerAudioData;
    public Watertank currentTank;
    public ScoreManager scoreManager;

    public int buildIndex_startScene = 0;
    public int buildIndex_mainMenu = 1;
    public int buildIndex_mainScene = 2;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
