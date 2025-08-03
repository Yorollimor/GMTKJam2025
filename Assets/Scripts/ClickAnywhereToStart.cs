using UnityEngine;
using UnityEngine.UI;

public class ClickAnywhereToStart : MonoBehaviour
{
    public Button b;

    private void Update()
    {
        if (Input.anyKeyDown)
            b.onClick.Invoke();
    }
}
