using UnityEngine;
using UnityEngine.UI;

public class SingleSelectionManager : MonoBehaviour, ISelectionManager
{
    public Button verifyButton;

    public GameObject successPanel;
    public GameObject failPanel;

    // 当前选中的对象
    private ColorBlock currentSelected;

    void Start()
    {
        verifyButton.interactable = false;

        successPanel.SetActive(false);
        failPanel.SetActive(false);
    }

    // 点击色块时调用
    public void SelectBlock(ColorBlock block)
    {
        // 如果之前已经选中过别人
        if (currentSelected != null)
        {
            currentSelected.Deselect();
        }

        // 更新当前选中的对象
        currentSelected = block;
        currentSelected.Select();

        verifyButton.interactable = true;
    }

    // 点击验证按钮
    public void VerifySelection()
    {
        if (currentSelected == null)
            return;

        if (currentSelected.isCorrect)
        {
            successPanel.SetActive(true);
        }
        else
        {
            failPanel.SetActive(true);
        }
    }
}