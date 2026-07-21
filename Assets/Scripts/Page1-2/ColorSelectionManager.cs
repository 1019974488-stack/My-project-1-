using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ColorSelectionManager : MonoBehaviour, ISelectionManager
{
    public GameObject successPanel;
    public GameObject failPanel;
    // 验证按钮（后面拖进来）
    public Button verifyButton;

    // 当前已选中的色块
    private List<ColorBlock> selectedBlocks = new List<ColorBlock>();

    void Start()
    {
        // 游戏开始按钮不可点击
        verifyButton.interactable = false;
        successPanel.SetActive(false);
        failPanel.SetActive(false);
    }

    public void SelectBlock(ColorBlock block)
    {
        if (block.selected)
            return;

        // 已经选满3个
        if (selectedBlocks.Count == 3)
        {
            foreach (ColorBlock b in selectedBlocks)
            {
                b.Deselect();
            }

            selectedBlocks.Clear();
            verifyButton.interactable = false;
        }

        block.Select();
        selectedBlocks.Add(block);

        if (selectedBlocks.Count == 3)
        {
            verifyButton.interactable = true;
        }

        Debug.Log("当前选中了：" + selectedBlocks.Count);
    }

        public void VerifySelection()
    {
        foreach (ColorBlock block in selectedBlocks)
        {
            if (!block.isCorrect)
            {
                failPanel.SetActive(true);
                Debug.Log("验证失败");
                return;
            }
        }

        successPanel.SetActive(true);
        Debug.Log("验证成功");
    }
}