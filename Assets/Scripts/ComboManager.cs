using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;
    private Coroutine comboPopCoroutine;


    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    public float comboDuration = 2.0f;
    private float comboTimer;
    private int currentCombo = 0;
    private int score = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (currentCombo > 0)
        {
            //comboText.text = "TEST COMBO x" + currentCombo;
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    public void AddScore(int basePoints)
    {
        if (comboTimer <= 0f)
            currentCombo = 0;

        currentCombo++;
        comboTimer = comboDuration;

        int pointsGained = basePoints * currentCombo;
        score += pointsGained;

        UpdateUI();
    }

    private void ResetCombo()
    {
        currentCombo = 0;
        comboText.text = "";
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;

        if (currentCombo > 1)
        {
            comboText.text = "Combo x" + currentCombo + "!";

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