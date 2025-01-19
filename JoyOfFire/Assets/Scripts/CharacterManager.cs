using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public List<CharacterAttributes> PlayerCharacters = new List<CharacterAttributes>();
    public List<CharacterAttributes> EnemyCharacters = new List<CharacterAttributes>();
    public List<CharacterAttributes> allCharacters = new List<CharacterAttributes>();
    public CharacterConfigurator configurator;
    public List<bool> isConfigured = new List<bool>();
    public GameObject PlayerCharacterHorizontalLayout;
    public GameObject EnemyCharacterHorizontalLayout;
    public GameObject PlayerCharacterPrefab;
    public GameObject EnemyCharacterPrefab;
    private int currentCharacterIndex;
    private CharacterAttributes currentCharacterClone;
    public Button startButton;
    public GameObject characterConfiguratorPanel;
    public List<Button> characterButtons;
    public List<Button> EnemyButtons;
    public static CharacterManager instance;
    public NewCharacterManager newCharacterManager;
    
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        newCharacterManager = FindObjectOfType<NewCharacterManager>();
        InitializeCharacters();
        InitializeButtons();
        startButton.interactable = false;
    }

    private void InitializeCharacters()
    {
        allCharacters = newCharacterManager.allCharacters;
        PlayerCharacters = newCharacterManager.allCharacters;
        foreach (Transform child in PlayerCharacterHorizontalLayout.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in EnemyCharacterHorizontalLayout.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < allCharacters.Count; i++)
        {   
            allCharacters[i].index = i;
            GameObject Character = Instantiate(PlayerCharacterPrefab, PlayerCharacterHorizontalLayout.transform);
            characterButtons.Add(Character.GetComponent<Button>());
            StartCoroutine(APIManager.instance.LoadImage(allCharacters[i].character_picture, Character.GetComponent<Image>()));
                
            // BattleManager.instance.characterButtons.Add(Character.GetComponent<Button>());
        }
        for (int i = 0; i < 1; i++)
        {
            

            CharacterAttributes character = new CharacterAttributes
            {
                index = PlayerCharacters.Count+i,
            };
            
            GameObject monster = Instantiate(EnemyCharacterPrefab, EnemyCharacterHorizontalLayout.transform);
            characterButtons.Add(monster.GetComponent<Button>());
            EnemyButtons.Add(monster.GetComponent<Button>());
            EnemyCharacters.Add(character);
            // BattleManager.instance.characterButtons.Add(monster.GetComponent<Button>());
            // BattleManager.instance.enemyButtons.Add(monster.GetComponent<Button>());
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
                if (button.CompareTag("Enemy"))
                {
                    OpenConfigurator(index);
                    characterConfiguratorPanel.SetActive(true);
                }
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
            isConfigured[0] = true;

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
