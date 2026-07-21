using UnityEngine;
using UnityEngine.EventSystems;

public class VerifyButton : MonoBehaviour, IPointerClickHandler
{
    private ColorSelectionManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<ColorSelectionManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.VerifySelection();
    }
}