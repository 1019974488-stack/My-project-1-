using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionManager : MonoBehaviour
{
    // 验证按钮（后面拖进来）
    public Button verifyButton;

    // 当前已选中的色块
    private List<ColorBlock> selectedBlocks = new List<ColorBlock>();

    void Start()
    {
        // 游戏开始按钮不可点击
        verifyButton.interactable = false;
    }

    public void SelectBlock(ColorBlock block)
    {

    }
}