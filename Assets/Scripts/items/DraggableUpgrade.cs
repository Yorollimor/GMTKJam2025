using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUpgrade : ItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public PlacableItem itemPrefab;

    private GameObject _clone;
    [SerializeField] private Canvas mainCanvas;
    public ContactFilter2D nonPlaceableLayer;
    private PlacableItem placeableItem;
    private NonPlaceableArea[] placedItemsArray;
    private SpriteRenderer sr;
    private MeshRenderer[] mr;
    [SerializeField] private PolygonCollider2D maskCollider; //used for checking placement area - is found in Watertank->Sprites->BG Mask
    private int samplePoints = 6;

    public int numberOfAllowedPurchases = -1; // Maximum number of times this item can be bought -1 = unlimited

    bool overlapped = true;
    Color right = Color.white;
    Color wrong = new Color(168, 0, 0);

    float pointerDownTime;
    Vector2 pointerDownPos;
    bool isDragging;
    bool isInFakeDrag = false;
    PointerEventData lastEvent;

    protected override void Start()
    {
        base.Start();
        maxBuyCount = numberOfAllowedPurchases;
    }

    private void Update()
    {
        if (isInFakeDrag)
        {
            lastEvent.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                EndFakeDrag();
            }
            else OnDrag_internal(lastEvent);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        OnDrag_internal(eventData);
    }
    public void OnDrag_internal(PointerEventData eventData)
    {
        if (IsSoldOut()) return;

        //set clone sprite position to mouse - UI space
        _clone.transform.position = eventData.position;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        if (placeableItem.GetComponent<SpriteRenderer>()) sr = placeableItem.GetComponent<SpriteRenderer>();
        else if(placeableItem.GetComponentInChildren<SpriteRenderer>()) sr = placeableItem.GetComponentInChildren<SpriteRenderer>();
        else if(placeableItem.GetComponentInChildren<MeshRenderer>()) mr = placeableItem.GetComponentsInChildren<MeshRenderer>();

        placeableItem.transform.position = worldPos;

        //checks if the item overlaps with any other items
        overlapped = placeableItem.GetComponentInChildren<NonPlaceableArea>() && placeableItem.GetComponentInChildren<NonPlaceableArea>().overlaps != 0;
        
        bool uiVisible = false;
        bool itemVisible = false;
        bool canBePlaced = false;

        // checks if a point of the collider is outside the watertank-area-mask
        if (!IsFullyInsideMask(placeableItem.GetComponentInChildren<NonPlaceableArea>()))
        {      
            if(!maskCollider.OverlapPoint(worldPos)) //checks if mouse position is outside the mask
            {
                uiVisible = true;
            }
            else // if the mouse position is inside the mask but the item is not fully inside the mask
            {
                itemVisible = true;
            }
        }
        else // if the item is fully inside the mask
        {
            itemVisible = true;
            canBePlaced = !overlapped;
            if (itemType == ItemType.Delete) canBePlaced = overlapped;
        }

        Color c = itemVisible ? canBePlaced ? right : wrong : new Color(0, 0, 0, 0);
        if (sr != null)
        {
            sr.color = c;
            if(itemType == ItemType.Delete)
            {
                if(canBePlaced) placeableItem.GetComponentInChildren<Animator>().SetBool("Hittable", true);
                else placeableItem.GetComponentInChildren<Animator>().SetBool("Hittable", false);
                sr.color = Color.white;
            }
        }
        else if (mr != null)
        {
            foreach (MeshRenderer meshRenderer in mr)
            {
                meshRenderer.material.color = c;
            }
        }

        c = uiVisible ? new Color(1, 1, 1, 1 ) : new Color(0, 0, 0, 0);
        _clone.GetComponent<Image>().color = c;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("PointerEvent: Begin Drag");
        isDragging = true;
        OnBeginDrag_internal(eventData);
    }

    public void OnBeginDrag_internal(PointerEventData eventData)
    {
        if (IsSoldOut())
        {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.upgradeFail);
            instance.start();
            return;
        }

        placedItemsArray = GameManager.Instance.currentTank.moveableObjectsParent.GetComponentsInChildren<NonPlaceableArea>();
        maskCollider = GameManager.Instance.currentTank.GetComponentInChildren<PolygonCollider2D>();

        foreach (NonPlaceableArea area in placedItemsArray)
        {
            area.GetComponent<ParticleSystem>().Play();
        }

        //creating a copy of the prefab and call the DraggedItem function from it's child component
        placeableItem = Instantiate(itemPrefab);
        placeableItem.GetComponentInChildren<NonPlaceableArea>().DraggedItem();
        foreach (Collider2D col in placeableItem.GetComponentsInChildren<Collider2D>())
        {
            if (col.gameObject.GetComponent<NonPlaceableArea>()) continue;
            col.enabled = false; // Disable colliders to prevent physics interactions during drag
        }

        // Create an Image clone
        _clone = new GameObject("ItemClone");
        _clone.transform.SetParent(mainCanvas.transform, false);
        _clone.transform.SetAsLastSibling();

        // Copy the sprite
        var image = _clone.AddComponent<Image>();
        image.sprite = itemUI.image.sprite;
        image.rectTransform.localScale = new Vector3(1f, 1f, 1f);

        // Prevent clone to block raycast
        var canvasGroup = _clone.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;

        overlapped = true; //assume overlap until proven otherwise
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("PointerEvent: End Drag");
        isDragging = false;
        OnEndDrag_internal(eventData);
    }
    public void OnEndDrag_internal(PointerEventData eventData)
    {
        if (IsSoldOut()) return;

        foreach (NonPlaceableArea area in placedItemsArray)
        {
            area.GetComponent<ParticleSystem>().Stop();
        }

        Destroy(_clone);
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 dropWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        //delete Hammer only
        if(itemType == ItemType.Delete)
        {
            FMOD.Studio.EventInstance deleteAudio;
            if (placeableItem.GetComponentInChildren<NonPlaceableArea>().IsOverlapping())
            {
                GameObject lastItem = placeableItem.GetComponentInChildren<NonPlaceableArea>().GetLastOverlap().gameObject.transform.parent.gameObject;
                Destroy(lastItem);
                deleteAudio = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.upgradeBuy);
            }
            else deleteAudio = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.upgradeFail);

            deleteAudio.start();
            Destroy(placeableItem.gameObject);
            return;
        }

        //Validation
        bool canAfford = GameManager.Instance.scoreManager.GetScore() >= (int)(price);

        FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(canAfford && !overlapped ? GameManager.Instance.playerAudioData.upgradeBuy : GameManager.Instance.playerAudioData.upgradeFail);
        instance.start();

        if (canAfford && !overlapped)
        {
            PlacableItem newItem = placeableItem;
            foreach (Collider2D col in placeableItem.GetComponentsInChildren<Collider2D>())
            {
                col.enabled = true; // Disable colliders to prevent physics interactions during drag
            }
            placeableItem.GetComponentInChildren<NonPlaceableArea>().DraggedPlaced();
            placeableItem = null;
            //TODO: Add validation

            //boundaries
            if (!IsFullyInsideMask(newItem.GetComponentInChildren<NonPlaceableArea>()))
                Destroy(newItem.gameObject);
            else
            {
                newItem.transform.SetParent(GameManager.Instance.currentTank.moveableObjectsParent);
                base.BuyItem();
            }

        }
        if(overlapped)
        {
            PlacedOverlapped();
        }
        if (placeableItem) Destroy(placeableItem.gameObject);
    }

    /// <summary>
    /// this function should generate a pop-up text or give some information about why it could not be placed
    /// </summary>
    private void PlacedOverlapped()
    {
        print("Can't place here!");
    }

    /// <summary>
    /// Checks if a collider is fully inside the placeable area mask. 
    /// </summary>
    /// <returns></returns>
    public bool IsFullyInsideMask(NonPlaceableArea objWithCollider)
    {
        Collider2D col = objWithCollider.GetComponent<Collider2D>();
        if (col == null) objWithCollider.GetComponentInChildren<Collider2D>();

        if (col == null || maskCollider == null)
        {
            Debug.LogWarning("Missing collider or mask reference.");
            return false;
        }

        List<Vector2> points = SampleColliderEdgePoints(col, samplePoints);

        foreach (Vector2 point in points)
        {
            if (!maskCollider.OverlapPoint(point))
            {
                return false; // Found a point outside the mask             
            }
        }

        return true; // All points are within the mask
    }

    /// <summary>
    /// Puts points on the edge of a circle or capsule collider, evenly spaced. 
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="pointCount"></param>
    /// <returns>A list of points in world-space along the colliders edge</returns>
    private List<Vector2> SampleColliderEdgePoints(Collider2D collider, int pointCount)
    {
        var points = new List<Vector2>();

        if (collider is CircleCollider2D circle)
        {
            Vector2 center = circle.transform.TransformPoint(circle.offset);
            float radius = circle.radius * Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);

            for (int i = 0; i < pointCount; i++)
            {
                float angle = 2 * Mathf.PI * i / pointCount;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                points.Add(center + dir * radius);
            }
        }
        else if (collider is CapsuleCollider2D capsule)
        {
            Vector2 center = capsule.transform.TransformPoint(capsule.offset);
            float width = capsule.size.x * 0.5f * capsule.transform.lossyScale.x;
            float height = capsule.size.y * 0.5f * capsule.transform.lossyScale.y;

            if (capsule.direction == CapsuleDirection2D.Vertical)
            {
                for (int i = 0; i < pointCount; i++)
                {
                    float angle = 2 * Mathf.PI * i / pointCount;
                    float x = Mathf.Cos(angle) * width;
                    float y = Mathf.Sin(angle) * height;
                    points.Add(center + new Vector2(x, y));
                }
            }
            else // Horizontal capsule
            {
                for (int i = 0; i < pointCount; i++)
                {
                    float angle = 2 * Mathf.PI * i / pointCount;
                    float x = Mathf.Cos(angle) * height;
                    float y = Mathf.Sin(angle) * width;
                    points.Add(center + new Vector2(x, y));
                }
            }
        }
        else
        {
            Debug.LogWarning("Unsupported collider type for sampling.");
        }

        return points;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerEvent: Down");
        pointerDownTime = eventData.clickTime;
        pointerDownPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            //do nothing
        }
        else if (GameManager.Instance.scoreManager.GetScore() < (int)(price))
        {
            FMOD.Studio.EventInstance deleteAudio = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.upgradeFail);
            deleteAudio.start();
        }
        else if (eventData.clickTime - pointerDownTime < 0.2f && Vector3.Distance(pointerDownPos, eventData.position) < 5)
        {
            Debug.Log("PointerEvent: Up Short");
            isInFakeDrag = true;
            GameManager.Instance.UIManager.SetBlockerActive(false);
            lastEvent = eventData;
            OnBeginDrag_internal(eventData);
        }
        else Debug.Log("PointerEvent: Up Long");

        pointerDownPos = Vector2.zero;
        pointerDownTime = 0;

    }

    private void EndFakeDrag()
    {
        GameManager.Instance.UIManager.SetBlockerActive(true);
        isInFakeDrag = false;
        OnEndDrag_internal(lastEvent);
    }
}
