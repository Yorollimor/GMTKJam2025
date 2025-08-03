using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScorePickUpUIController : MonoBehaviour
{
    public static ScorePickUpUIController Instance { get; private set; }

    [Header("Popup Settings")]
    public GameObject popupPrefab;
    public Sprite myScoreIconSprite;
    public int maxPopups = 5;
    public float popupDuration = 3f;

    [Header("Background Sprites")]
    public Sprite[] backgroundSprites = new Sprite[3]; // Array for 3 background sprites
    public bool useRandomOrder = true; // Toggle between random and sequential
    
    private int currentBackgroundIndex = 0; // For sequential order
    private readonly Queue<GameObject> activePopups = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple ScorePickUpUIController instances detected! Destroying extra ones.");
            Destroy(gameObject);
        }
    }

    public void ShowScorePopup(string scoreText, Sprite scoreIcon = null)
    {
        GameObject newPopup = Instantiate(popupPrefab, transform);
        newPopup.transform.SetAsLastSibling(); // Ensure it appears on top in UI stack

        // Set Score Text
        TMP_Text textComponent = newPopup.GetComponentInChildren<TMP_Text>();
        if (textComponent)
        {
            textComponent.text = scoreText;
        }

        // Set Icon if exists
        Image scoreImage = newPopup.transform.Find("ScoreIcon")?.GetComponent<Image>();
        if (scoreImage && scoreIcon != null)
        {
            scoreImage.sprite = scoreIcon;
            scoreImage.enabled = true;
        }
        else if (scoreImage)
        {
            scoreImage.enabled = false;
        }

        // Set Random Background Sprite
        SetRandomBackground(newPopup);

        activePopups.Enqueue(newPopup);
        if (activePopups.Count > maxPopups)
        {
            Destroy(activePopups.Dequeue());
        }

        // Start Fade Out
        StartCoroutine(FadeOutAndDestroy(newPopup));
    }

    private void SetRandomBackground(GameObject popup)
    {
        // Check if we have background sprites
        if (backgroundSprites == null || backgroundSprites.Length == 0)
        {
            Debug.LogWarning("No background sprites assigned to ScorePickUpUIController!");
            return;
        }

        // Find the background image component (assuming it's the main Image on the popup)
        Image backgroundImage = popup.GetComponent<Image>();
        
        // If not found, try to find it by name or tag
        if (backgroundImage == null)
        {
            backgroundImage = popup.transform.Find("Background")?.GetComponent<Image>();
        }
        
        // If still not found, try getting the first Image component
        if (backgroundImage == null)
        {
            backgroundImage = popup.GetComponentInChildren<Image>();
        }

        if (backgroundImage != null)
        {
            Sprite selectedSprite;
            
            if (useRandomOrder)
            {
                // Random selection
                int randomIndex = Random.Range(0, backgroundSprites.Length);
                selectedSprite = backgroundSprites[randomIndex];
            }
            else
            {
                // Sequential selection
                selectedSprite = backgroundSprites[currentBackgroundIndex];
                currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundSprites.Length;
            }

            // Only set sprite if it's not null
            if (selectedSprite != null)
            {
                backgroundImage.sprite = selectedSprite;
            }
            else
            {
                Debug.LogWarning($"Background sprite at index is null!");
            }
        }
        else
        {
            Debug.LogWarning("Could not find background Image component on popup prefab!");
        }
    }

    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);
        if (popup == null) yield break;

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogWarning("CanvasGroup not found on popup prefab!");
            Destroy(popup);
            yield break;
        }

        float fadeDuration = 1f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if(canvasGroup)canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ScorePickUpUIController.Instance.ShowScorePopup("High Score: 9999");
        }

        Destroy(popup);
    }

    // Optional: Method to manually set background sprites via code
    public void SetBackgroundSprites(Sprite[] sprites)
    {
        backgroundSprites = sprites;
    }

    // Optional: Method to add a single background sprite
    public void AddBackgroundSprite(Sprite sprite)
    {
        if (backgroundSprites == null)
        {
            backgroundSprites = new Sprite[1];
            backgroundSprites[0] = sprite;
        }
        else
        {
            System.Array.Resize(ref backgroundSprites, backgroundSprites.Length + 1);
            backgroundSprites[backgroundSprites.Length - 1] = sprite;
        }
    }
}