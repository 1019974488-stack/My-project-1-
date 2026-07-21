using UnityEngine;
using UnityEngine.EventSystems;

public class ColorBlock : MonoBehaviour,
IPointerEnterHandler,
IPointerExitHandler,
IPointerClickHandler
{
    public bool selected = false;
    public bool isCorrect = false;

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

    private ISelectionManager manager;

    void Start()
    {
        manager = GetComponentInParent<ISelectionManager>();

        if (manager == null)
        {
            manager = FindFirstObjectByType<ColorSelectionManager>();

            if (manager == null)
            {
                manager = FindFirstObjectByType<SingleSelectionManager>();
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        manager.SelectBlock(this);
    }

    public void Select()
    {
        selected = true;
        transform.localScale = Vector3.one * 0.9f;
    }

    public void Deselect()
    {
        selected = false;
        transform.localScale = Vector3.one;
    }
}