using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUpgrade : ItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;

    private GameObject _clone;
    [SerializeField] private Canvas mainCanvas;
    public ContactFilter2D nonPlaceableLayer;
    private GameObject placeableItem;
    private NonPlaceableArea[] placedItemsArray;
    private SpriteRenderer sr;

    /// <summary>
    /// Must be in this order: Top Left, Top Right, Bottom Left, Bottom Right. 
    /// </summary>
    private Transform[] boundaryPointsForPlacement;

    bool overlapped = false;

    public void OnDrag(PointerEventData eventData)
    {
        //set clone sprite position to mouse - UI space
        _clone.transform.position = eventData.position;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        if (placeableItem.GetComponent<SpriteRenderer>()) sr = placeableItem.GetComponent<SpriteRenderer>();
        else sr = placeableItem.GetComponentInChildren<SpriteRenderer>();

        placeableItem.transform.position = worldPos;
        if (placeableItem.GetComponentInChildren<NonPlaceableArea>() && placeableItem.GetComponentInChildren<NonPlaceableArea>().overlaps != 0 )
        {
           sr.color = new Color(168,0,0);
            print(placeableItem.transform.name);
            overlapped = true;
        }
        else
        {
           sr.color = Color.white;
            overlapped = false;
        }

        if (IsObjectOutOfBoundaries(placeableItem.transform))
        {
           sr.color = new Color(0, 0, 0, 0);
            _clone.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            if(!overlapped)sr.color = Color.white;
            _clone.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        boundaryPointsForPlacement = GameManager.Instance.currentTank.GetPlacementBoundaries();
        placedItemsArray = GameManager.Instance.currentTank.moveableObjectsParent.GetComponentsInChildren<NonPlaceableArea>();

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
            if (IsObjectOutOfBoundaries(newItem.transform))
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

    private void PlacedOverlapped()
    {
        print("Can't place here!");
    }

    private bool IsObjectOutOfBoundaries(Transform objTransform)
    {
        return (objTransform.localPosition.x < boundaryPointsForPlacement[0].localPosition.x
                || objTransform.localPosition.y > boundaryPointsForPlacement[0].localPosition.y
                || objTransform.localPosition.x > boundaryPointsForPlacement[1].localPosition.x
                || objTransform.localPosition.y > boundaryPointsForPlacement[1].localPosition.y
                || objTransform.localPosition.x < boundaryPointsForPlacement[2].localPosition.x
                || objTransform.localPosition.y < boundaryPointsForPlacement[2].localPosition.y
                || objTransform.localPosition.x > boundaryPointsForPlacement[3].localPosition.x
                || objTransform.localPosition.y < boundaryPointsForPlacement[3].localPosition.y);
    }
}
