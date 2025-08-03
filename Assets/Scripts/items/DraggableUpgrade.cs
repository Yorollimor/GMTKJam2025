using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUpgrade : ItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;

    private GameObject _clone;
    [SerializeField] private Canvas mainCanvas;
    
    public void OnDrag(PointerEventData eventData)
    {
        //set clone sprite position to mouse
        _clone.transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        Destroy(_clone);
        
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 dropWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);


        //TODO: Add validation
        bool canAfford = GameManager.Instance.scoreManager.GetScore() >= (int)(price);

        FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(canAfford ? GameManager.Instance.playerAudioData.upgradeBuy : GameManager.Instance.playerAudioData.upgradeFail);
        instance.start();

        if (canAfford)
        {
            Transform itemParent = GameManager.Instance.currentTank.transform.GetChild(0);
            GameObject newItem = Instantiate(itemPrefab, dropWorldPosition, Quaternion.identity, itemParent);

            //TODO: Add validation

            //boundaries
            if (newItem.transform.localPosition.x < -10f 
                || newItem.transform.localPosition.x > 10f
                || newItem.transform.localPosition.y < -3f 
                || newItem.transform.localPosition.y > 15f)
                Destroy(newItem);
            else
                base.BuyItem();
        }
    }
}
