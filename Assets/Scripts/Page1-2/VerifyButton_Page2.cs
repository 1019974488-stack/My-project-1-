using UnityEngine;
using UnityEngine.EventSystems;

public class VerifyButton_Page2 : MonoBehaviour, IPointerClickHandler
{
    private SingleSelectionManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<SingleSelectionManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.VerifySelection();
    }
}