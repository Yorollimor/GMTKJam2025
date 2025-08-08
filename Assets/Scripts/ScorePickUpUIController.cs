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
    private int currentPopUps = 0;
    public float popupDuration = 3f;
    
    public float yIncrementalMultiplier = 1.1f;
    public float randomRotationRange = 5f;

    [Header("Background Sprites")]
    public Sprite[] backgroundSprites = new Sprite[3]; // Array for 3 background sprites
    public bool useRandomOrder = true; // Toggle between random and sequential
    
    private int currentBackgroundIndex = 0; // For sequential order
    private readonly Queue<GameObject> activePopups = new();

    private GameObject spawnLocation;

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

    private void Start()
    {
        spawnLocation = GameManager.Instance.currentTank.GetScoreSpawnLocation();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            ShowScorePopup_WorldSpace("test");
        }
    }

    public void ShowScorePopup(string scoreText, Sprite scoreIcon = null)
    {
        currentPopUps++;
        
        GameObject newPopup = Instantiate(popupPrefab, spawnLocation.transform);

        Vector2 positionIncrement = new Vector2(Random.Range(popupPrefab.GetComponent<RectTransform>().rect.width/2, popupPrefab.GetComponent<RectTransform>().rect.width*1.5f), popupPrefab.GetComponent<RectTransform>().rect.height * currentPopUps);

        newPopup.transform.localPosition = positionIncrement;
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

    public void ShowScorePopup_WorldSpace(string scoreText, Sprite scoreIcon = null)
    {
        currentPopUps++;

        GameObject newPopup = Instantiate(popupPrefab, spawnLocation.transform);

        Vector2 positionIncrement = new Vector2(Random.Range(popupPrefab.GetComponent<RectTransform>().rect.width / 3, popupPrefab.GetComponent<RectTransform>().rect.width * 1.2f), 0);
        newPopup.transform.localPosition = positionIncrement;
        newPopup.transform.localRotation = Quaternion.Euler(0,0,Random.Range(-randomRotationRange, randomRotationRange));
        newPopup.transform.SetAsLastSibling(); // Ensure it appears on top in UI stack

        int counter = 1;
        foreach (GameObject obj in activePopups)
        {
            obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y + popupPrefab.GetComponent<RectTransform>().rect.height * yIncrementalMultiplier * counter);
        }

        // Set Score Text
        TextMeshPro textComponent = newPopup.GetComponentInChildren<TextMeshPro>();
        if (textComponent)
        {
            textComponent.text = scoreText;
        }

        // Set Icon if exists
        SpriteRenderer scoreImage = newPopup.transform.Find("ScoreIcon")?.GetComponent<SpriteRenderer>();
        if (scoreImage && scoreIcon != null)
        {
            scoreImage.sprite = scoreIcon;
            scoreImage.enabled = true;
        }
        else if (scoreImage)
        {
            //scoreImage.enabled = false;
        }

        // Set Random Background Sprite
        SetRandomBackground_WorldSpace(newPopup);

        activePopups.Enqueue(newPopup);
       
        /*
       if (activePopups.Count > maxPopups)
        {
            activePopups.Clear();
        }*/

        // Start Fade Out
        StartCoroutine(FadeOutAndDestroy_WorldSpace(newPopup));
    }

    private void SetRandomBackground_WorldSpace(GameObject popup)
    {
        // Check if we have background sprites
        if (backgroundSprites == null || backgroundSprites.Length == 0)
        {
            Debug.LogWarning("No background sprites assigned to ScorePickUpUIController!");
            return;
        }

        // Find the background image component (assuming it's the main Image on the popup)
        SpriteRenderer backgroundImage = popup.GetComponent<SpriteRenderer>();

        // If not found, try to find it by name or tag
        if (backgroundImage == null)
        {
            backgroundImage = popup.transform.Find("Background")?.GetComponent<SpriteRenderer>();
        }

        // If still not found, try getting the first Image component
        if (backgroundImage == null)
        {
            backgroundImage = popup.GetComponentInChildren<SpriteRenderer>();
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
            if(canvasGroup) canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ScorePickUpUIController.Instance.ShowScorePopup("High Score: 9999");
        }

        Destroy(popup);
        currentPopUps--;
    }
    private IEnumerator FadeOutAndDestroy_WorldSpace(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);
        if (popup == null) yield break;

        SpriteRenderer sr = popup.GetComponent<SpriteRenderer>();
        SpriteRenderer sr_icon = popup.transform.Find("ScoreIcon").GetComponent<SpriteRenderer>();
        TextMeshPro text = popup.GetComponentInChildren<TextMeshPro>();

        float fadeDuration = 1f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (sr) sr.color = new Color(0,0,0,Mathf.Lerp(1f, 0f, timer / fadeDuration));
            if (sr_icon) sr_icon.color = new Color(0, 0, 0, Mathf.Lerp(1f, 0f, timer / fadeDuration));
            if (text) text.color = new Color(0, 0, 0, Mathf.Lerp(1f, 0f, timer / fadeDuration));
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ScorePickUpUIController.Instance.ShowScorePopup("High Score: 9999");
        }
        Destroy(activePopups.Dequeue());
        currentPopUps--;
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