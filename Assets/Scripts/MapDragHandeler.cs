using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MapDragHandeler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 gap;//écart entre la position de la map et celle de la souris au moment du clic

    public void OnBeginDrag(PointerEventData eventData)
    {
        gap = GetComponent<RectTransform>().anchoredPosition - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().anchoredPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + gap;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}