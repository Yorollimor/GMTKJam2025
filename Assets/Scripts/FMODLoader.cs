using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class FMODLoader : MonoBehaviour
{
    public static FMODLoader Instance { get; private set; }
    bool loadingBanks = false;

    public UnityEvent OnBankLoadingStarted;
    public UnityEvent OnBankLoadingFinished;

    private string[] bankNames = new string[] { "Master", "Master.strings" };

    private void Awake()
    {
        if(FMODLoader.Instance != null)
        {
            Destroy(this);
        }
        else
        {
            FMODLoader.Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

#if UNITY_EDITOR
        LoadFMOD();
#endif

    }

    // Update is called once per frame
    void Update()
    {

        if (loadingBanks)
        {
            bool allBanksLoaded = true;
            foreach (string bank in bankNames)
            {
                if (!RuntimeManager.HasBankLoaded(bank)) allBanksLoaded = false;
            }
            if (allBanksLoaded)
            {
                loadingBanks = false;
                OnBankLoadingFinished.Invoke();
            }
        }
    }

    public void LoadFMOD()
    {
        loadingBanks = true;
        foreach(string bank in bankNames)
        {
            RuntimeManager.LoadBank(bank, true);
        }
        OnBankLoadingStarted.Invoke();

    }
}
