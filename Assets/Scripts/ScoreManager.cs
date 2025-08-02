using System.Collections;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{

    private int score = 0;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    private Coroutine comboPopCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        GameManager.Instance.scoreManager = this;
    }

    public void UpdateScore(int addScore)
    {
        scoreText.text = "Score: " + score;
        score += addScore;

    }

    public void UpdateCombo(int combo)
    {
        if (combo > 1)
        {
            comboText.text = "Combo x" + combo + "!";

            // Play Pop Animation
            if (comboPopCoroutine != null)
                StopCoroutine(comboPopCoroutine);

            comboPopCoroutine = StartCoroutine(ComboPopEffect());
        }
        else
        {
            comboText.text = "";
        }
    }

    private IEnumerator ComboPopEffect()
    {
        float duration = 0.2f;
        float scaleAmount = 1.5f;

        Vector3 originalScale = comboText.transform.localScale;
        Vector3 targetScale = originalScale * scaleAmount;

        // Scale Up
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            comboText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Scale Down
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            comboText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        comboText.transform.localScale = originalScale;
    }

}
