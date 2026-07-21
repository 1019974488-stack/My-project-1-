using UnityEngine;
using UnityEngine.EventSystems;

public class RightButtonController : MonoBehaviour,
    IPointerClickHandler
{
    public SlotMachineController slotMachine;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotMachine != null)
        {
            slotMachine.StopSpin();
        }
    }
}