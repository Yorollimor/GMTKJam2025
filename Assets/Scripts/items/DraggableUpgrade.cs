using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUpgrade : ItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;

    private GameObject _clone;
    private RectTransform _sourceRectTransform;
    private RectTransform _cloneRectTransform;
    private Canvas _parentCanvas;
    
    public void OnDrag(PointerEventData eventData)
    {
        //set clone sprite position to mouse
        _clone.transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //instantiate a clone sprite
        _clone = new GameObject("ItemVisualCopy_" + base.name);
        SpriteRenderer spriteRenderer = _clone.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemUI.image.sprite;
        // _clone.transform.localScale = new Vector3(100f, 100f, 100f);
          
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(_clone);
        
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 dropWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);

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
