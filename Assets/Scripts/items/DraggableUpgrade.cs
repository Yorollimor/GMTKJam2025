using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUpgrade : ItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;

    private GameObject _clone;
    
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        
        //set clone sprite position to mouse
        _clone.transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        
        //instantiate a clone sprite
        _clone = new GameObject("ItemVisualCopy_" + base.name);
        SpriteRenderer spriteRenderer = _clone.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemUI.image.sprite;;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(_clone);
        Instantiate(itemPrefab, eventData.position, Quaternion.identity);
        
        //TODO: Add validation
        base.BuyItem();
        Debug.Log("OnEndDrag");
    }
}
