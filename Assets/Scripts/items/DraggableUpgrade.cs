using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUpgrade : ItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{
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
        Debug.Log("OnEndDrag");
    }
}
