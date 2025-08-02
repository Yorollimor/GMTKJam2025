using System.Collections;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private int displayedScore = 0; // Track what's currently shown
    private int highScore = 0;
    private bool hasNewHighScoreBeenNotified = false;

    public TextMeshProUGUI scoreText;
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

    private void Start()
    {
        GameManager.Instance.scoreManager = this;
        UpdateScoreDisplay(0); // Initialize display
    }

    public void UpdateScore(int addScore)
    {
        int oldScore = score;
        score += addScore;
        
        // Play score increase sound
        PlayScoreIncreaseSound();
        
        // Start flip animation from old score to new score
        if (scoreFlipCoroutine != null)
            StopCoroutine(scoreFlipCoroutine);
            
        scoreFlipCoroutine = StartCoroutine(FlipScoreAnimation(oldScore, score));

        // High Score Check & Popup Trigger
        if (score > highScore)
        {
            highScore = score;
            ScorePickUpUIController.Instance.ShowScorePopup("+" + addScore.ToString());
        }
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
            float scaleY = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 2)) * 0.8f + 0.2f; // Scale between 0.2 and 1.0
            scoreText.transform.localScale = new Vector3(1f, scaleY, 1f);
            
            UpdateScoreDisplay(currentDisplayScore);
            
            yield return null;
        }
        
        // Ensure final values are set
        scoreText.transform.localScale = Vector3.one;
        UpdateScoreDisplay(toScore);
        displayedScore = toScore;
    }

    private void UpdateScoreDisplay(int scoreValue)
    {
        string formattedScore = scoreValue.ToString("D6"); // Format with leading zeros
        string spacedScore = "";
        
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
        
        scoreText.text = "" + spacedScore;
    }

    private void PlayScoreIncreaseSound()
    {
        try
        {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(scoreIncreaseSoundEvent);
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