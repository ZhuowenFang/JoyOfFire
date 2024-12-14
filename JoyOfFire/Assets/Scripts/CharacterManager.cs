using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public List<CharacterAttributes> allCharacters = new List<CharacterAttributes>();
    public CharacterConfigurator configurator;
    public List<bool> isConfigured = new List<bool>();

    private int currentCharacterIndex;
    private CharacterAttributes currentCharacterClone;
    public Button startButton;
    public GameObject characterConfiguratorPanel;
    public List<Button> characterButtons;

    void Start()
    {
        InitializeCharacters();
        InitializeButtons();
        startButton.interactable = false;
    }

    private void InitializeCharacters()
    {
        for (int i = 0; i < 6; i++)
        {
            CharacterAttributes character = new CharacterAttributes
            {
                index = i
            };
            allCharacters.Add(character);
            isConfigured.Add(false);
            Debug.Log($"初始化角色索引 {i}");
        }
    }

    private void InitializeButtons()
    {
        foreach (var button in characterButtons)
        {
            int index = characterButtons.IndexOf(button);  // 避免闭包问题
            button.onClick.RemoveAllListeners();  // 确保没有预设事件
            button.onClick.AddListener(() =>
            {
                OpenConfigurator(index);
                characterConfiguratorPanel.SetActive(true);
            });
            Debug.Log($"绑定角色按钮 {button.name} 的点击事件，索引 {index}");
        }
    }

    public void OpenConfigurator(int characterIndex)
    {
        currentCharacterIndex = characterIndex;
        currentCharacterClone = allCharacters[characterIndex].Clone();
        configurator.LoadCharacter(currentCharacterClone);
    }

    public void SaveConfiguration()
    {
        if (configurator.AreAllInputsValid())
        {
            configurator.SaveCharacter();
            allCharacters[currentCharacterIndex] = currentCharacterClone.Clone();
            isConfigured[currentCharacterIndex] = true;

            Debug.Log($"角色索引 {currentCharacterIndex} 已配置完成！");
            CheckAllConfigured();
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
            startButton.interactable = true;
            Debug.Log("所有角色已配置完成，可以开始游戏！");
        }
    }
}
