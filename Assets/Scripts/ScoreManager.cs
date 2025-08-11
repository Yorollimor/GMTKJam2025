using System.Collections;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{

    [Header("Combo Effect Settings")]
    public bool enableComboEffect = true;
    public enum ComboEffectType { Bounce, Shake, FadeAndScale }
    public ComboEffectType comboEffectType = ComboEffectType.Bounce;
    public ParticleSystem comboParticles; // Optional 
    public GameObject comboParticlesPrefab; // Add this missing variable


    private int score = 0;
    private int displayedScore = 0; // Track what's currently shown
    private int highScore = 0;
    private bool hasNewHighScoreBeenNotified = false;

    public TextMeshProUGUI comboText;

    // Flip Clock Effect Variables
    public float flipDuration = 0.3f;
    public AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Sound and Visual Settings
    [Header("Audio")]
    public string scoreIncreaseSoundEvent = "event:/SFX/UI/ScoreIncrease"; // FMOD event path

    [Header("Digit Spacing")]
    public float digitSpacing = 0.1f; // Adjustable spacing between digits

    private Coroutine comboPopCoroutine;
    private Coroutine scoreFlipCoroutine;

    public UnityEvent<int> OnScoreChanged;

    public void UpdateScore(int addScore)
    {
        int oldScore = score;
        score += addScore;

        // Play score increase sound
        if (addScore > 0) PlayScoreIncreaseSound();

        // Start flip animation from old score to new score
        if (scoreFlipCoroutine != null)
            StopCoroutine(scoreFlipCoroutine);

        scoreFlipCoroutine = StartCoroutine(FlipScoreAnimation(oldScore, score));

        // High Score Check & Popup Trigger
        if (score > highScore && addScore > 0)
        {
            highScore = score;
            ScorePickUpUIController.Instance.ShowScorePopup_WorldSpace("+" + addScore.ToString());
        }
        OnScoreChanged.Invoke(score);
    }


    private IEnumerator FlipScoreAnimation(int fromScore, int toScore)
    {
        float timer = 0f;

        while (timer < flipDuration)
        {
            timer += Time.deltaTime;
            float t = timer / flipDuration;
            float curveValue = flipCurve.Evaluate(t);

            // Interpolate between old and new score
            int currentDisplayScore = Mathf.RoundToInt(Mathf.Lerp(fromScore, toScore, curveValue));

            // Add vertical flip effect by scaling Y
            float scaleY = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 2)) * 0.4f + 0.1f; // Scale between 0.2 and 1.0
            GameManager.Instance.currentTank.scoreText.transform.localScale = new Vector3(1f, scaleY, 1f);

            UpdateScoreDisplay(currentDisplayScore);

            yield return null;
        }

        // Ensure final values are set
        GameManager.Instance.currentTank.scoreText.transform.localScale = Vector3.one;
        UpdateScoreDisplay(toScore);
        displayedScore = toScore;
    }



    private void UpdateScoreDisplay(int scoreValue)
    {
        string formattedScore = scoreValue.ToString("D7"); // Format with leading zeros
        /*string spacedScore = "";
        
        // Add spacing between digits using a simpler approach
        for (int i = 0; i < formattedScore.Length; i++)
        {
            spacedScore += formattedScore[i];
            
            // Add spaces after each digit except the last one
            if (i < formattedScore.Length - 1)
            {
                // Use regular spaces with adjustable count based on digitSpacing
                int spaceCount = Mathf.RoundToInt(digitSpacing * 10f); // Convert to space count
                spaceCount = Mathf.Max(1, spaceCount); // Ensure at least 1 space
                spacedScore += new string(' ', spaceCount);
            }
        }
        Spacing handled in text mesh pro 
        */

        GameManager.Instance.currentTank.scoreText.text = formattedScore;
    }

    private void PlayScoreIncreaseSound()
    {
        try
        {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.upgradeBuy);
            instance.start();
            instance.release(); // Release the instance after starting
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Could not play score increase sound: " + e.Message);
        }
    }

    public void UpdateCombo(int combo)
    {

        comboText.transform.localScale = Vector3.one;

        if (combo > 1)
        {
            comboText.gameObject.SetActive(true); // Show text in fixed UI position
            comboText.text = $"Combo x{combo}!";

            if (combo < 3)
                comboText.color = Color.yellow;
            else if (combo < 4)
                comboText.color = new Color(1f, 0.5f, 0f); // orange
            else
                comboText.color = Color.red;

            comboText.fontMaterial.SetFloat("_OutlineWidth", 0.2f); // adjust thickness
            comboText.fontMaterial.SetColor("_OutlineColor", Color.white);

            // Stop any current animation before starting a new one
            if (comboPopCoroutine != null)
                StopCoroutine(comboPopCoroutine);




            // Choose a random animation effect
            int effectIndex = Random.Range(0, 4); // 0 = Bounce, 1 = Shake, 2 = Scale, 3 = Fade
            switch (effectIndex)
            {
                case 0:
                    comboPopCoroutine = StartCoroutine(FadeEffect());
                    break;
                case 1:
                    comboPopCoroutine = StartCoroutine(ShakeEffect());

                    break;
                case 2:
                    comboPopCoroutine = StartCoroutine(ScaleEffect());
                    break;
                    //case 3:
                    // comboPopCoroutine = StartCoroutine(BounceEffect());
                    // break;
            }

            if (comboParticlesPrefab != null)
            {
                Debug.Log("Spawning particle!");

                // Convert UI position to world position in Camera space
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, comboText.transform.position);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));

                GameObject particles = Instantiate(comboParticlesPrefab, worldPos, Quaternion.identity);
                Destroy(particles, 2f);
            }

        }
        else
        {
            comboText.text = "";
            comboText.gameObject.SetActive(false); // Hide when combo ends
            comboText.fontMaterial.SetFloat("_OutlineWidth", 0f); // remove outline when combo ends

        }

    }

    public void SpendPoints(int amount)
    {
        UpdateScore(-amount);
    }

    private IEnumerator ComboPopEffect()
    {
        Vector3 originalScale = comboText.transform.localScale;
        Quaternion originalRotation = comboText.transform.localRotation;
        Color originalColor = comboText.color;

        // Pick a random animation
        int effectType = Random.Range(0, 4); // 0=ScaleBounce, 1=Shake, 2=Fade, 3=Spin

        switch (effectType)
        {
            case 0: // Scale bounce
                yield return Spin(originalRotation);

                break;
            case 1: // Shake
                yield return Shake(originalScale);
                break;
            case 2: // Fade
                yield return Fade(originalColor);
                break;
                //  case 3: // Spin
                // yield return ScaleBounce(originalScale);
                // break;
        }

        // Reset to original
        comboText.transform.localScale = originalScale;
        comboText.transform.localRotation = originalRotation;
        comboText.color = originalColor;
    }

    private IEnumerator ScaleBounce(Vector3 originalScale)
    {
        float duration = 0.25f;
        float scaleAmount = 1.6f;

        // Scale up
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            comboText.transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleAmount, t / duration);
            yield return null;
        }
        // Scale back
        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            comboText.transform.localScale = Vector3.Lerp(originalScale * scaleAmount, originalScale, t / duration);
            yield return null;
        }
    }

    private IEnumerator Shake(Vector3 originalScale)
    {
        float duration = 0.3f;
        float magnitude = 5f;

        Vector3 originalPos = comboText.rectTransform.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Random.Range(-magnitude, magnitude);
            float offsetY = Random.Range(-magnitude, magnitude);
            comboText.rectTransform.anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }
        comboText.rectTransform.anchoredPosition = originalPos;
    }

    private IEnumerator Fade(Color originalColor)
    {
        float duration = 0.5f;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, Mathf.PingPong(t * 2, 1));
            comboText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }

    private IEnumerator Spin(Quaternion originalRotation)
    {
        float duration = 0.3f;
        float angle = 15f;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float rotation = Mathf.Sin(t * Mathf.PI * 2) * angle;
            comboText.transform.localRotation = originalRotation * Quaternion.Euler(0, 0, rotation);
            yield return null;
        }
    }

    private IEnumerator BounceEffect()
    {
        Vector3 originalPos = comboText.rectTransform.localPosition;
        float bounceHeight = 20f;
        float duration = 0.2f;

        for (int i = 0; i < 3; i++)
        {
            comboText.rectTransform.localPosition = originalPos + Vector3.up * bounceHeight;
            yield return new WaitForSeconds(duration);
            comboText.rectTransform.localPosition = originalPos;
            yield return new WaitForSeconds(duration);
        }
    }

    private IEnumerator ShakeEffect()
    {
        Vector3 originalPos = comboText.rectTransform.localPosition;
        float shakeAmount = 5f;
        float duration = 0.05f;

        for (int i = 0; i < 8; i++)
        {
            comboText.rectTransform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return new WaitForSeconds(duration);
        }
        comboText.rectTransform.localPosition = originalPos;
    }

    private IEnumerator ScaleEffect()
    {
        Vector3 originalScale = comboText.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        float duration = 0.2f;

        // Scale Up
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            comboText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }

        // Scale Down
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            comboText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            yield return null;
        }
    }

    private IEnumerator FadeEffect()
    {
        Color originalColor = comboText.color;
        Color fadeColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
        float duration = 0.5f;

        // Fade Out
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            comboText.color = Color.Lerp(originalColor, fadeColor, t / duration);
            yield return null;
        }

        // Fade Back In
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            comboText.color = Color.Lerp(fadeColor, originalColor, t / duration);
            yield return null;
        }
    }

    public int GetScore()
    {
        return score;
    }

}