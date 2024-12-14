using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public List<CharacterAttributes> allCharacters = new List<CharacterAttributes>(); // 存储6个角色的属性
    public CharacterConfigurator configurator; // 引用配置界面脚本
    public List<bool> isConfigured = new List<bool>();  // 配置状态列表

    private int currentCharacterIndex; // 当前选中的角色索引
    private CharacterAttributes currentCharacterClone;
    public Button startButton;  // 开始按钮
    

    void Start()
    {
        // 初始化6个角色，每个角色拥有独立的属性对象
        for (int i = 0; i < 6; i++)
        {
            allCharacters.Add(new CharacterAttributes());
            isConfigured.Add(false);  // 初始状态为未配置

        }
        startButton.interactable = false;

    }

    public void OpenConfigurator(int characterIndex)
    {
        currentCharacterIndex = characterIndex; // 保存当前角色索引
        currentCharacterClone = allCharacters[characterIndex].Clone(); // 创建角色属性的深拷贝
        configurator.LoadCharacter(currentCharacterClone); // 将拷贝加载到配置界面
    }

    // 保存配置
    public void SaveConfiguration()
    {
        if (configurator.AreAllInputsValid())
        {
            configurator.SaveCharacter();
            isConfigured[currentCharacterIndex] = true;  // 标记该角色已配置
            Debug.Log($"角色 {currentCharacterIndex} 已配置完成！");
            CheckAllConfigured();  // 检查是否全部配置完成
        }
        else
        {
            Debug.LogError("角色配置无效，无法保存！");
        }
    }
    private void CheckAllConfigured()
    {
        if (!isConfigured.Contains(false))
        {
            startButton.interactable = true;  // 启用开始按钮
            Debug.Log("所有角色已配置完成，可以开始游戏！");
        }
    }
}
