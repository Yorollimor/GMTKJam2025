using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUpgrade : ItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;
    
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //TODO: Add validation
        base.BuyItem();
        Debug.Log("OnEndDrag");
    }
}
