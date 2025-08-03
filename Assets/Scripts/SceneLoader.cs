using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
 public void StartSceneAfterLoad()
    {
        SceneManager.LoadScene("WaterTest");
    }
}
