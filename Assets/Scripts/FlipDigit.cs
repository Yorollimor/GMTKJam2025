using UnityEngine;
using TMPro;

public class FlipDigit : MonoBehaviour
{
    public RectTransform digitsPanel; // The vertical container with 0–9 digits
    public float flipSpeed = 10f;
    private int currentDigit = 0;

    private float targetY;
    private float digitHeight;

    void Start()
    {
        digitHeight = ((RectTransform)digitsPanel.GetChild(0)).rect.height;
        SetDigitInstant(currentDigit); // Reset
    }

    public void SetDigit(int newDigit)
    {
        if (newDigit < 0 || newDigit > 9) return;

        currentDigit = newDigit;
        targetY = newDigit * digitHeight;
    }

    public void SetDigitInstant(int newDigit)
    {
        SetDigit(newDigit);
        digitsPanel.anchoredPosition = new Vector2(0, targetY);
    }

    void Update()
    {
        Vector2 pos = digitsPanel.anchoredPosition;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * flipSpeed);
        digitsPanel.anchoredPosition = pos;
    }
}
