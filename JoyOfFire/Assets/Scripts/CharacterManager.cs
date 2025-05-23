using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleCharacterManager : MonoBehaviour
{
    public List<CharacterAttributes> PlayerCharacters = new List<CharacterAttributes>();
    public List<MonsterAttributes> EnemyCharacters = new List<MonsterAttributes>();
    public List<MonsterAttributes> Enemys = new List<MonsterAttributes>();
    [SerializeReference]
    public List<ICharacter> allCharacters = new List<ICharacter>();
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
    public static BattleCharacterManager instance;
    public NewCharacterManager newCharacterManager;
    public GameObject speedBackGround;
    public GameObject BattleManagerGameObject;
    
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        newCharacterManager = FindObjectOfType<NewCharacterManager>();
        InitializeCharacters();
        InitializeButtons();
    }

    private void InitializeCharacters()
    {
        allCharacters = newCharacterManager.allCharacters;

        foreach (Transform child in PlayerCharacterHorizontalLayout.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in EnemyCharacterHorizontalLayout.transform)
        {
            Destroy(child.gameObject);
        }

        characterButtons.Clear();
        EnemyButtons.Clear();
        PlayerCharacters.Clear();
        EnemyCharacters.Clear();

        Debug.Log($"All Characters Count: {allCharacters.Count}");
        

        for (int i = 0; i < allCharacters.Count; i++)
        {   
            if (allCharacters[i] is CharacterAttributes character)
            {
                character.energy = allCharacters[0].energy;
                GameObject playerObj = Instantiate(PlayerCharacterPrefab, PlayerCharacterHorizontalLayout.transform);
                playerObj.transform.Find("Image").gameObject.SetActive(false);
                playerObj.transform.Find("fill").gameObject.SetActive(false);

                characterButtons.Add(playerObj.GetComponent<Button>());
                DoubleClickListener dblClick = playerObj.AddComponent<DoubleClickListener>();
                dblClick.onDoubleClick += () =>
                {
                    Debug.Log($"双击角色 {character.characterName} 的 index: {character.index}");
                    DetailManager.instance.isEnemy = false;
                    DetailManager.instance.UpdateCharacterButtons();
                    DetailManager.instance.ShowDetail(character, false);
                };
                PlayerCharacters.Add(character);
                character.index = PlayerCharacters.Count - 1;
                // StartCoroutine(APIManager.instance.LoadImage(character.character_picture, playerObj.GetComponent<Image>()));
                StartCoroutine(ImageCache.GetTexture(character.character_picture, (Texture2D texture) =>
                {
                    if (texture != null)
                    {
                        playerObj.GetComponent<Image>().sprite = Sprite.Create(texture, 
                            new Rect(0, 0, texture.width, texture.height), 
                            new Vector2(0.5f, 0.5f));
                    }
                }));
                Debug.Log(character.index);
            }
            else if (allCharacters[i] is MonsterAttributes enemy)
            {
                GameObject enemyObj = Instantiate(EnemyCharacterPrefab, EnemyCharacterHorizontalLayout.transform);
                enemyObj.transform.Find("Image").gameObject.SetActive(false);
                enemyObj.transform.Find("fill").gameObject.SetActive(false);
                enemyObj.GetComponent<Image>().sprite = Resources.Load<Sprite>($"EnemyPics/{enemy.monsterId}");
                characterButtons.Add(enemyObj.GetComponent<Button>());
                DoubleClickListener dblClick = enemyObj.AddComponent<DoubleClickListener>();
                dblClick.onDoubleClick += () =>
                {
                    Debug.Log($"双击角色 {enemy.characterName} 的 index: {enemy.index}");
                    DetailManager.instance.isEnemy = true;
                    DetailManager.instance.UpdateCharacterButtons();
                    DetailManager.instance.ShowDetail(enemy, true);
                };
                EnemyButtons.Add(enemyObj.GetComponent<Button>());

                enemy.index = i;
                EnemyCharacters.Add(enemy);

                BaseEnemy enemyComponent = enemyObj.GetComponent<BaseEnemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.enemyAttributes = enemy;
                }
                Debug.Log(enemy.index);
            }
        }
        
        foreach (var enemy in EnemyCharacters)
        {
            Debug.Log($"敌人 {enemy.characterName} 的 index: {enemy.index}");
        }
    }

    private async void InitializeButtons()
    {
        foreach (var button in characterButtons)
        {
            int index = characterButtons.IndexOf(button);
            button.onClick.RemoveAllListeners();
            // button.onClick.AddListener(() =>
            // {
            //     if (button.CompareTag("Enemy"))
            //     {
            //         // OpenConfigurator(index);
            //         characterConfiguratorPanel.SetActive(true);
            //     }
            // });
            Debug.Log($"绑定角色按钮 {button.name} 的点击事件，索引 {index}");
        }
        await Task.Delay(500);
        speedBackGround.SetActive(true);
        BattleManagerGameObject.SetActive(true);

    }

    // public void OpenConfigurator(int characterIndex)
    // {
    //     currentCharacterIndex = characterIndex;
    //     currentCharacterClone = allCharacters[characterIndex].Clone();
    //     configurator.LoadCharacter(currentCharacterClone);
    // }

    // public void SaveConfiguration()
    // {
    //     if (configurator.AreAllInputsValid())
    //     {
    //         configurator.SaveCharacter();
    //         allCharacters[currentCharacterIndex] = currentCharacterClone.Clone();
    //         EnemyCharacters[0] = currentCharacterClone.Clone();
    //         EnemyButtons[0].GetComponent<BaseEnemy>().enemyAttributes = currentCharacterClone.Clone();
    //         isConfigured[0] = true;
    //
    //         Debug.Log($"角色索引 {currentCharacterIndex} 已配置完成！");
    //         CheckAllConfigured();
    //     }
    //     else
    //     {
    //         Debug.LogError("角色配置无效，无法保存！");
    //     }
    // }

    private void CheckAllConfigured()
    {
        if (!isConfigured.Contains(false))
        {
            startButton.interactable = true;
            Debug.Log("所有角色已配置完成，可以开始游戏！");
        }
    }
    
}
