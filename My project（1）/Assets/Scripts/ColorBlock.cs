using UnityEngine;
using UnityEngine.EventSystems;

public class ColorBlock : MonoBehaviour,
IPointerEnterHandler,
IPointerExitHandler,
IPointerClickHandler
{
    public bool selected = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
        {
            transform.localScale = Vector3.one * 0.9f;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            transform.localScale = Vector3.one;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selected = true;
        transform.localScale = Vector3.one * 0.9f;

        Debug.Log(gameObject.name + " ÒÑÑ¡ÖÐ");


    }
}