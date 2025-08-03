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
        
        Debug.Log("mousePos: " + mouseScreenPos);
        Debug.Log("worldPos: " + dropWorldPosition); //this is returning 0,0,0
        
        Instantiate(itemPrefab, dropWorldPosition, Quaternion.identity);
        
        //TODO: Add validation
        base.BuyItem();
    }
}
