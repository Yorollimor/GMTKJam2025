using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DraggableUpgrade : ItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;

    private GameObject _clone;
    [SerializeField] private Canvas mainCanvas;
    public ContactFilter2D nonPlaceableLayer;
    private GameObject placeableItem;
    private NonPlaceableArea[] placedItemsArray;
    private SpriteRenderer sr;
    private MeshRenderer[] mr;
    [SerializeField] private PolygonCollider2D maskCollider; //used for checking placement area - is found in Watertank->Sprites->BG Mask
    private int samplePoints = 6;

    bool overlapped = false;

    public void OnDrag(PointerEventData eventData)
    {
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
        if (placeableItem.GetComponentInChildren<NonPlaceableArea>() && placeableItem.GetComponentInChildren<NonPlaceableArea>().overlaps != 0 )
        {
           overlapped = true;
        }
        else
        {
           overlapped = false;
        }

        // checks if a point of the collider is outside the watertank-area-mask
        if (!IsFullyInsideMask(placeableItem.GetComponentInChildren<NonPlaceableArea>()))
        {      
            if(!maskCollider.OverlapPoint(worldPos)) //checks if mouse position is outside the mask
            {
                SetItemInvisible(true);
                _clone.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else // if the mouse position is inside the mask but the item is not fully inside the mask
            {
                SetItemInvisible(false);
                SetItemColorRed();
                _clone.GetComponent<Image>().color = new Color(0,0,0,0);
            }
        }
        else // if the item is fully inside the mask
        {
            if (overlapped)
            {
                SetItemInvisible(false);
                SetItemColorRed();
            }
            else
            {
                if (sr != null) sr.color = Color.white;
                else if (mr != null)
                {
                    foreach (MeshRenderer meshRenderer in mr)
                    {
                        meshRenderer.material.color = Color.white;
                    }
                }
            }
            _clone.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
    }

    private void SetItemColorRed()
    {
        if (sr != null)
        {
            sr.color = sr.color = new Color(168, 0, 0);
        }
        else if (mr != null)
        {
            foreach (MeshRenderer meshRenderer in mr)
            {
                meshRenderer.material.color = new Color(168, 0, 0);
            }
        }
    }

    /// <summary>
    /// "true" sets the item invisible, "false" sets it visible again.
    /// </summary>
    /// <param name="yes"></param>
    private void SetItemInvisible(bool yes=true)
    {
        if(yes)
        {
            if (sr != null)
                sr.color = new Color(0, 0, 0, 0);
            else if (mr != null)
            {
                foreach (MeshRenderer meshRenderer in mr)
                {
                    meshRenderer.material.color = new Color(0, 0, 0, 0);
                }
            }
        }
        else
        {
            if (sr != null)
                sr.color = Color.white;
            else if (mr != null)
            {
                foreach (MeshRenderer meshRenderer in mr)
                {
                    meshRenderer.material.color = Color.white;
                }
            }
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        placedItemsArray = GameManager.Instance.currentTank.moveableObjectsParent.GetComponentsInChildren<NonPlaceableArea>();
        maskCollider = GameManager.Instance.currentTank.GetComponentInChildren<PolygonCollider2D>();

        foreach (NonPlaceableArea area in placedItemsArray)
        {
            area.GetComponent<ParticleSystem>().Play();
        }

        //creating a copy of the prefab and call the DraggedItem function from it's child component
        placeableItem = Instantiate(itemPrefab);
        placeableItem.GetComponentInChildren<NonPlaceableArea>().DraggedItem();

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


    }

    public void OnEndDrag(PointerEventData eventData)
    {

        foreach (NonPlaceableArea area in placedItemsArray)
        {
            area.GetComponent<ParticleSystem>().Stop();
        }

        Destroy(_clone);
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 dropWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        //Validation
        bool canAfford = GameManager.Instance.scoreManager.GetScore() >= (int)(price);

        FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(canAfford ? GameManager.Instance.playerAudioData.upgradeBuy : GameManager.Instance.playerAudioData.upgradeFail);
        instance.start();

        if (canAfford && !overlapped)
        {
            GameObject newItem = placeableItem;
            placeableItem.GetComponentInChildren<NonPlaceableArea>().DraggedPlaced();
            placeableItem = null;
            //TODO: Add validation

            //boundaries
            if (!IsFullyInsideMask(newItem.GetComponentInChildren<NonPlaceableArea>()))
                Destroy(newItem);
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
}
