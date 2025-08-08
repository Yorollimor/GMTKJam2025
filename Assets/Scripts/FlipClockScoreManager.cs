using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlipClockScoreManager : MonoBehaviour
{
    private int score = 0;
    private int displayedScore = 0;
    private int highScore = 0;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    
    [Header("Flip Clock Settings")]
    public int numberOfDigits = 6;
    public float flipDuration = 0.4f;
    public float digitFlipDelay = 0.05f; // Delay between each digit flip
    public AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private List<DigitFlipper> digitFlippers = new List<DigitFlipper>();
    private Coroutine scoreFlipCoroutine;
    private Coroutine comboPopCoroutine;

    [System.Serializable]
    public class DigitFlipper
    {
        public int currentDigit;
        public int targetDigit;
        public float flipProgress;
        public bool isFlipping;
    }

    private void Start()
    {
        //GameManager.Instance.scoreManager = this;
        InitializeDigitFlippers();
        UpdateScoreDisplay();
    }

    private void InitializeDigitFlippers()
    {
        digitFlippers.Clear();
        for (int i = 0; i < numberOfDigits; i++)
        {
            digitFlippers.Add(new DigitFlipper());
        }
    }

    public void UpdateScore(int addScore)
    {
        int oldScore = score;
        score += addScore;
        
        if (scoreFlipCoroutine != null)
            StopCoroutine(scoreFlipCoroutine);
            
        scoreFlipCoroutine = StartCoroutine(AnimateScoreChange(oldScore, score));

        // High Score Check
        if (score > highScore)
        {
            highScore = score;
            ScorePickUpUIController.Instance.ShowScorePopup_WorldSpace("+" + addScore.ToString());
        }
    }

    private IEnumerator AnimateScoreChange(int fromScore, int toScore)
    {
        // Convert scores to digit arrays
        int[] fromDigits = GetDigitArray(fromScore);
        int[] toDigits = GetDigitArray(toScore);
        
        // Set up digit flippers
        List<int> digitsToFlip = new List<int>();
        for (int i = 0; i < numberOfDigits; i++)
        {
            digitFlippers[i].currentDigit = fromDigits[i];
            digitFlippers[i].targetDigit = toDigits[i];
            digitFlippers[i].isFlipping = fromDigits[i] != toDigits[i];
            digitFlippers[i].flipProgress = 0f;
            
            if (digitFlippers[i].isFlipping)
            {
                digitsToFlip.Add(i);
            }
        }

        // Start flipping digits with staggered timing (right to left for natural counting effect)
        for (int i = digitsToFlip.Count - 1; i >= 0; i--)
        {
            int digitIndex = digitsToFlip[i];
            StartCoroutine(FlipDigit(digitIndex));
            
            if (digitFlipDelay > 0)
                yield return new WaitForSeconds(digitFlipDelay);
        }

        // Wait for all flips to complete
        yield return new WaitForSeconds(flipDuration);
        
        // Ensure final state
        displayedScore = toScore;
        UpdateScoreDisplay();
    }

    private IEnumerator FlipDigit(int digitIndex)
    {
        DigitFlipper flipper = digitFlippers[digitIndex];
        float timer = 0f;

        while (timer < flipDuration)
        {
            timer += Time.deltaTime;
            flipper.flipProgress = timer / flipDuration;
            
            UpdateScoreDisplay();
            yield return null;
        }

        flipper.flipProgress = 1f;
        flipper.isFlipping = false;
        flipper.currentDigit = flipper.targetDigit;
    }

    private int[] GetDigitArray(int number)
    {
        int[] digits = new int[numberOfDigits];
        for (int i = numberOfDigits - 1; i >= 0; i--)
        {
            digits[i] = number % 10;
            number /= 10;
        }
        return digits;
    }

    private void UpdateScoreDisplay()
    {
        string scoreString = "Score: ";
        
        for (int i = 0; i < numberOfDigits; i++)
        {
            DigitFlipper flipper = digitFlippers[i];
            
            if (flipper.isFlipping)
            {
                // Create flip effect with character interpolation
                float curveValue = flipCurve.Evaluate(flipper.flipProgress);
                
                // For visual effect, show transitioning digit
                if (curveValue < 0.5f)
                {
                    scoreString += flipper.currentDigit.ToString();
                }
                else
                {
                    scoreString += flipper.targetDigit.ToString();
                }
            }
            else
            {
                scoreString += flipper.currentDigit.ToString();
            }
        }
        
        // Apply scaling effect during flip
        float maxFlipProgress = 0f;
        foreach (var flipper in digitFlippers)
        {
            if (flipper.isFlipping)
                maxFlipProgress = Mathf.Max(maxFlipProgress, flipper.flipProgress);
        }
        
        if (maxFlipProgress > 0f)
        {
            float scaleY = 1f - (Mathf.Sin(maxFlipProgress * Mathf.PI) * 0.3f);
            scoreText.transform.localScale = new Vector3(1f, scaleY, 1f);
        }
        else
        {
            scoreText.transform.localScale = Vector3.one;
        }
        
        scoreText.text = scoreString;
    }

    public void UpdateCombo(int combo)
    {
        if (combo > 1)
        {
            comboText.text = "Combo x" + combo + "!";

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