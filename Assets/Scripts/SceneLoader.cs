using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void StartSceneAfterLoad()
    {
        SceneManager.LoadScene("GameScene");
        Debug.Log("Loading GameScene after FMOD is loaded.");
    }
}
