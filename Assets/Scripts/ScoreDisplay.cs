using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public FlipDigit[] digits; // Set this array from the Inspector

    public void UpdateScoreDisplay(int score)
    {
        string scoreStr = score.ToString().PadLeft(digits.Length, '0');

        for (int i = 0; i < digits.Length; i++)
        {
            int digitValue = int.Parse(scoreStr[i].ToString());
            digits[i].SetDigit(digitValue);
        }
    }
}