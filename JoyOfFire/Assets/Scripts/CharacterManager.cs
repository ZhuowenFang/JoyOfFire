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
    

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            allCharacters.Add(new CharacterAttributes());
            isConfigured.Add(false);

        }
        startButton.interactable = false;

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
            isConfigured[currentCharacterIndex] = true; 
            Debug.Log($"角色 {currentCharacterIndex} 已配置完成！");
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
