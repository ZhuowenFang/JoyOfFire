using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public class LevelPrerequisite
    {
        public List<string> mustNotTriggered;
        public List<string> mustTriggered;
    }
    public bool hasTriggered = false;

    public GameObject player;
    public List<HexagonEvent> hexagons = new List<HexagonEvent>();
    public GameObject CharacterButton;
    public GameObject EventPanel;
    public Text EventTitle;
    public Text CharacterSelectionEventTitle;
    public Text EventDescription;
    public Button ChoiceA;
    public Button ChoiceB;
    public Button ChoiceC;
    public Button ChoiceDetail;
    private string level;
    public List<List<string>> LevelPoolList = new List<List<string>>();
    // private List<string> levelPool;
    public ClassManager.LevelResponse levelResponse;
    public GameObject CharacterPanel;
    public GameObject CharacterSelectionLayout;
    public Button ConfirmButton;
    private string APIresponse;
    public Dictionary<string, JObject> optionResults = new Dictionary<string, JObject>();
    public GameObject RewardPanel;
    public GameObject RewardPrefab;
    public GameObject HorizontalLayoutGroup;
    public string levelOrder = "1";
    public string NextStage = null;
    public Dictionary<string,float> CurrentRewards = new Dictionary<string, float>();
    
    public Dictionary<string,float> Blessings = new Dictionary<string, float>();
    
    public static EventManager instance;
    
    private HashSet<string> triggeredLevels = new HashSet<string>();
    public GameObject HintLayout;
    public GameObject BlessHint;
    public GameObject ToolButton;
    public GameObject BlessButton;
    public GameObject CharacterSelectionPrefab;

    private Coroutine typingCoroutine;
    private bool skipTyping = false;
    public float characterDelay = 0.05f;

    
    public static Dictionary<string, LevelPrerequisite> levelPrerequisites = new Dictionary<string, LevelPrerequisite>()
    {
        { "1-1", new LevelPrerequisite { mustNotTriggered = new List<string> { "1-4" } } },
        { "1-4", new LevelPrerequisite { mustNotTriggered = new List<string> { "1-1" } } },
        { "1-5", new LevelPrerequisite { mustNotTriggered = new List<string> { "1-6" } } },

        { "1-6", new LevelPrerequisite { mustTriggered = new List<string> { "1-3" } } },
        // { "1-7", new LevelPrerequisite { mustTriggered = new List<string> { "1-6" } } },
        { "1-8", new LevelPrerequisite { mustTriggered = new List<string> { "1-7" } } },
        { "1-9", new LevelPrerequisite { mustTriggered = new List<string> { "1-8" } } },
        { "1-10", new LevelPrerequisite { mustTriggered = new List<string> { "1-9" } } },
        { "2-7", new LevelPrerequisite { mustTriggered = new List<string> { "2-6" } } },
        { "2-8", new LevelPrerequisite { mustTriggered = new List<string> { "2-6" } } },
        { "2-9", new LevelPrerequisite { mustTriggered = new List<string> { "2-6" } } },
        { "2-10", new LevelPrerequisite { mustTriggered = new List<string> { "2-6" } } },
        { "2-11", new LevelPrerequisite { mustTriggered = new List<string> { "2-6" } } },
    };
    public static LevelPrerequisite GetPrerequisite(string level)
    {
        if (levelPrerequisites.ContainsKey(level))
            return levelPrerequisites[level];
        return null;
    }
    public bool CheckPrerequisites(LevelPrerequisite prereq)
    {
        if (prereq.mustNotTriggered != null)
        {
            foreach (string lvl in prereq.mustNotTriggered)
            {
                if (triggeredLevels.Contains(lvl))
                {
                    Debug.Log($"前置条件不满足：关卡 {lvl} 已触发。");
                    return false;
                }
                
            }
        }
    
        if (prereq.mustTriggered != null)
        {
            foreach (string lvl in prereq.mustTriggered)
            {
                if (!triggeredLevels.Contains(lvl))
                {
                    Debug.Log($"前置条件不满足：关卡 {lvl} 尚未触发。");
                    return false;
                }
            }
        }
    
        return true;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void InitializeLevelPool()
    {
        List<string> levelPool = new List<string>();

        for (int i = 1; i <= 4; i++)
        {
            levelPool.Add($"1-{i}");
        }

        for (int i = 12; i <= 13; i++)
        {
            levelPool.Add($"1-{i}");
        }

        for (int i = 15; i <= 21; i++)
        {
            levelPool.Add($"1-{i}");
        }
        LevelPoolList.Add(levelPool);
        List<string> levelPool1 = new List<string>();
        for (int i = 1; i <= 5; i++)
        {
            levelPool1.Add($"2-{i}");
        }
        levelPool1.Add($"2-12");

        LevelPoolList.Add(levelPool1);
        List<string> levelPool2 = new List<string>();

        for (int i = 1; i <= 5; i++)
        {
            levelPool2.Add($"3-{i}");
        }
        levelPool2.Add($"3-7");
        LevelPoolList.Add(levelPool2);
        List<string> levelPool3 = new List<string>();
        for (int i = 10; i <= 11; i++)
        {
            levelPool3.Add($"3-{i}");
        }
        LevelPoolList.Add(levelPool3);
        List<string> levelPool4 = new List<string>();

        for (int i = 13; i <= 14; i++)
        {
            levelPool4.Add($"3-{i}");
        }
        LevelPoolList.Add(levelPool4);
        List<string> levelPool5 = new List<string>();
        for (int i = 16; i <= 19; i++)
        {
            levelPool5.Add($"3-{i}");
        }
        LevelPoolList.Add(levelPool5);
        
        Debug.LogError(LevelPoolList.Count);
        
        
    }

    async void Start()
    {
        foreach (var hex in FindObjectsOfType<HexagonEvent>())
        {
            hexagons.Add(hex);
        }
        InitializeLevelPool();
        // await Task.Delay(1000);
        createInitialItem();
    }
    
    public void ReloadEvents()
    {
        foreach (var hex in hexagons.ToList())
        {
            hexagons.Remove(hex);
            // Destroy(hex.gameObject);
        }
        foreach (var hex in FindObjectsOfType<HexagonEvent>())
        {
            hexagons.Add(hex);
        }
    }

    void Update()
    {
        CheckPlayerPosition();
    }

    void CheckPlayerPosition()
    {
        Vector3 playerPosition = player.transform.position;

        foreach (var hex in hexagons.ToList())
        {
            float distance = Vector3.Distance(playerPosition, hex.center);

            if (distance <= hex.radius)
            {
                if (!hex.playerInside)
                {
                    hex.playerInside = true;
                    Time.timeScale = 0;
                    TriggerHexEvent(hex);
                }
            }
            else
            {
                hex.playerInside = false;
            }
        }
    }


    void TriggerHexEvent(HexagonEvent hex)
    {
        if (hex.isTransition)
        {
            Debug.LogError(hex.transitionMapIndex);
            Debug.LogError(hex.respawnPointIndex);
            MapTransitionManager.instance.TransitionMapWithFade(hex.transitionMapIndex, hex.respawnPointIndex);
            Time.timeScale = 1;
            return;
        }
        // else
        // {
        //     Time.timeScale = 1;
        //     return;
        // }
        if (string.IsNullOrEmpty(hex.eventNumber))
        {
            // Debug.LogError(hex.eventNumber);
            int randomIndex = UnityEngine.Random.Range(0, LevelPoolList[hex.randomPoolIndex].Count);
            Debug.LogError(randomIndex);
            Debug.Log(LevelPoolList[hex.randomPoolIndex].Count);
            
            if(NextStage != "")
            {
                level = NextStage;
                levelOrder = NextStage.Split('-')[0];
                NextStage = null;

            }
            else
            {
                level = LevelPoolList[hex.randomPoolIndex][randomIndex];
                LevelPoolList[hex.randomPoolIndex].RemoveAt(randomIndex);
                levelOrder = level.Split('-')[0];
            }
            if (GetPrerequisite(level) != null)
            {
                if (!CheckPrerequisites(GetPrerequisite(level)))
                {
                    Debug.Log($"前置条件不满足，无法触发关卡 {level}。");
                    TriggerHexEvent(hex);
                    return;
                }
            }
        }
        else
        {
            level = hex.eventStage + "-" + hex.eventNumber;
            levelOrder = hex.eventStage;
            if (GetPrerequisite(level) != null)
            {
                if (!CheckPrerequisites(GetPrerequisite(level)))
                {
                    Debug.Log($"固定关卡 {level} 前置条件不满足，事件不触发。");
                    
                    Time.timeScale = 1;
                    return;
                }
            }
        }
        StartCoroutine(FadeOutAndDeactivate(3f, "你触发了事件"));

        APIManager.instance.GetLevelData(
            level,
            onSuccess: (response) =>
            {
                Debug.Log(level);
                Debug.Log($"成功响应：{response}");
                levelResponse = JsonUtility.FromJson<ClassManager.LevelResponse>(response);
                CharacterPanel.SetActive(true);
                foreach (Transform child in CharacterSelectionLayout.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                {
                    GameObject newCharacter = Instantiate(CharacterSelectionPrefab, CharacterSelectionLayout.transform);
                    newCharacter.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
                    // StartCoroutine(APIManager.instance.LoadImage(character.character_picture, newCharacter.GetComponent<Image>()));
                    StartCoroutine(ImageCache.GetTexture(character.character_picture, (Texture2D texture) =>
                    {
                        if (texture != null)
                        {
                            newCharacter.GetComponent<Image>().sprite = Sprite.Create(texture, 
                                new Rect(0, 0, texture.width, texture.height), 
                                new Vector2(0.5f, 0.5f));
                        }
                    }));
                    newCharacter.transform.Find("Name").GetComponent<Text>().text = character.characterName;
                    newCharacter.transform.Find("Level").GetComponent<Text>().text = "等级：" + character.level.ToString();
                    ButtonGroup.instance.buttons.Add(newCharacter.GetComponent<Button>());
                    ButtonGroup.instance.InitializeButtons();
                }
                CharacterButton.SetActive(false);
                ToolButton.SetActive(false);
                BlessButton.SetActive(false);
                CharacterSelectionEventTitle.text = level + " " + levelResponse.data.levelInfo.level_name;
                triggeredLevels.Add(level);
                ConfirmButton.onClick.AddListener(() =>
                {
                    ParseLevelData();
                    ButtonGroup.instance.buttons.Clear();
                    
                    CharacterPanel.SetActive(false);
                });

            },
            onError: (error) =>
            {
                Debug.LogError($"请求失败：{error}");
            }
        );
        hexagons.Remove(hex);
        Destroy(hex.gameObject);
    }



    private IEnumerator TypeText(string fullText)
    {
        EventDescription.text = "";
        int i = 0;
        while (i < fullText.Length)
        {
            if (skipTyping)
            {
                // 用户点击后直接显示全部文本
                EventDescription.text = fullText;
                yield break;
            }
            EventDescription.text += fullText[i];
            i++;
            yield return new WaitForSecondsRealtime(characterDelay);
        }
        // 最后确保显示完整文本
        EventDescription.text = fullText;
    }

    public void OnDescriptionClicked()
    {
        Debug.Log("Clicked");
        skipTyping = true;
    }

    public void ParseLevelData()
    {

        EventPanel.SetActive(true);
        EventTitle.text = level + " " + levelResponse.data.levelInfo.level_name;
        // EventDescription.text = levelResponse.data.levelInfo.level_desc;
        

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        skipTyping = false;
        typingCoroutine = StartCoroutine(TypeText(levelResponse.data.levelInfo.level_desc));

        ParseOptionResult("option_a_res", levelResponse.data.levelInfo.option_a_res);
        ParseOptionResult("option_b_res", levelResponse.data.levelInfo.option_b_res);
        ParseOptionResult("option_c_res", levelResponse.data.levelInfo.option_c_res);

        SetupChoices();
    }

    private void ParseOptionResult(string optionKey, string rawJson)
    {
        try
        {
            if (!string.IsNullOrEmpty(rawJson) && rawJson != "nan")
            {
                JObject optionResult = JObject.Parse(rawJson);
                optionResults[optionKey] = optionResult;
                
            }
            else
            {
                optionResults[optionKey] = null;
                Debug.LogWarning($"{optionKey} is not available.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON Parsing Error for {optionKey}: {e.Message}");
        }
    }

    private void SetupChoices()
    {
        SetupChoice(ChoiceA, "option_a_res", levelResponse.data.levelInfo.option_a_text);
        SetupChoice(ChoiceB, "option_b_res", levelResponse.data.levelInfo.option_b_text);
        SetupChoice(ChoiceC, "option_c_res", levelResponse.data.levelInfo.option_c_text);
    }

    private void SetupChoice(Button choiceButton, string optionKey, string optionText)
    {
        if (string.IsNullOrEmpty(optionText) || optionText == "nan" || !optionResults.ContainsKey(optionKey) || optionText == "\n")
        {
            choiceButton.gameObject.SetActive(false);
            return;
        }
    
        bool hasTool = true;
        JObject optionResult = optionResults[optionKey];
        if (optionResult != null && optionResult["tool_required"] != null)
        {
            JToken toolRequiredToken = optionResult["tool_required"];
            if (toolRequiredToken.HasValues && toolRequiredToken.Type == JTokenType.Object)
            {
                hasTool = toolRequiredToken.Children<JProperty>().Any(prop => InventoryManager.instance.activeItems.ContainsKey(prop.Name));
            }
        }
    
        choiceButton.interactable = hasTool;
        choiceButton.gameObject.SetActive(true);
        choiceButton.GetComponentInChildren<Text>().text = optionText;

        choiceButton.onClick.RemoveAllListeners();
        choiceButton.onClick.AddListener(() => HandleChoice(optionKey, choiceButton));

    }

    private void HandleChoice(string optionKey,Button choiceButton)
    {
        if (optionResults.TryGetValue(optionKey, out JObject optionResult) && optionResult != null)
        {
            ChoiceDetail.gameObject.SetActive(true);
            ChoiceA.gameObject.SetActive(false);
            ChoiceB.gameObject.SetActive(false);
            ChoiceC.gameObject.SetActive(false);
            ChoiceDetail.onClick.RemoveAllListeners();
            Text[] texts = ChoiceDetail.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = choiceButton.GetComponentInChildren<Text>().text;
                texts[1].text = optionResult["success_copywriting"].ToString();

            }
            else
            {
                Debug.LogError("Not enough Text components found in ChoiceDetail.");
            }

            ChoiceDetail.onClick.AddListener( () =>
            {
                ChoiceDetail.gameObject.SetActive(false);
                
                if (optionResult["next_stage"] != null)
                {
                    NextStage = optionResult["next_stage"].ToString();
                }
                if(NextStage.Split('-')[0] != levelOrder)
                {
                    switch (NextStage.Split('-')[0])
                    {
                        case "2":
                            MapTransitionManager.instance.TransitionMapWithFade(3,3);
                            break;
                        case "3":
                            MapTransitionManager.instance.TransitionMapWithFade(4,4);
                            break;
        
                    }
                }
                string experienceToAdd = "";
                if (optionKey == "option_a_res")
                {
                    experienceToAdd = levelResponse.data.levelInfo.option_a_experience;
                    Debug.LogError(levelResponse.data.level_order);
                    Debug.LogError(levelResponse.data.level_num);

                    if (levelResponse.data.level_order == 1 && levelResponse.data.level_num == 7)
                    {
                        MapTransitionManager.instance.TransitionMapWithFade(1,1);
                    } else if (levelResponse.data.level_order == 1 && levelResponse.data.level_num == 9)
                    {
                        MapTransitionManager.instance.TransitionMapWithFade(2,2);
                    }
                    
                }
                else if (optionKey == "option_b_res")
                {
                    experienceToAdd = levelResponse.data.levelInfo.option_b_experience;
                }
                else if (optionKey == "option_c_res")
                {
                    experienceToAdd = levelResponse.data.levelInfo.option_c_experience;
                }
                if (!string.IsNullOrEmpty(experienceToAdd))
                {
                    foreach (CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                    {
                        String experience = experienceToAdd.Replace("{{character}}", character.characterName);
                        if (character.experience == null)
                        {
                            character.experience = new List<string>();
                        }
                        character.experience.Add(experience);
                        ClassManager.CharacterUpdateAttributes data = new ClassManager.CharacterUpdateAttributes
                        {
                            user_id = character.user_id,
                            character_id = character.character_id,
                            experience = character.experience
                        };
                        string jsonData = JsonUtility.ToJson(data);
                        APIManager.instance.StoreCharacter(
                            jsonData,
                            onSuccess: (response) =>
                            {
                                Debug.Log($"成功响应：{response}");
                                // character.UpdateAttributes();
                            },
                            onError: (error) =>
                            {
                                Debug.LogError($"请求失败：{error}");
                            }
                        );

                    }
                }
                
                if (optionResult["monsters"].Count() != 0 && optionResult["monsters"] is JObject monsters)
                {
                    
                    if (optionResult["rewards"] != null && optionResult["rewards"] is JObject rewards)
                    {
                        foreach (var reward in rewards)
                        {
                            CurrentRewards[reward.Key] = reward.Value.Value<float>();
                        }
                    }
                    
                    int totalAPICalls = monsters.Count;
                    int finishedAPICalls = 0;

                    foreach (var monster in monsters)
                    {
                        APIManager.instance.GetMonsterData(levelOrder, monster.Key, 
                            (monsters1) =>
                            {
                                foreach (var monster1 in monsters1)
                                {
                                    for (int i = 0; i < monster.Value.Value<int>(); i++)
                                    {
                                        MonsterAttributes cloneMonster = monster1.Clone();
                                        NewCharacterManager.instance.AddCharacter(cloneMonster);
                                    }
                                }
                                finishedAPICalls++;
                                if (finishedAPICalls >= totalAPICalls)
                                {
                                    SceneManager.LoadScene("Battle", LoadSceneMode.Additive);
                                    EventPanel.SetActive(false);
                                }
                            },
                            (error) =>
                            {
                                Debug.LogError($"加载怪物失败: {error}");
                            });
                        
                    }

                }
                else
                {
                    EventPanel.SetActive(false);
                }
                

                if (optionResult["treasure"].Count() != 0 && optionResult["treasure"] is JObject treasures)
                {
                    RewardPanel.SetActive(true);
                    
                    foreach (Transform child in HorizontalLayoutGroup.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    foreach (var treasure in treasures)
                    {
                        GameObject newReward = Instantiate(RewardPrefab, HorizontalLayoutGroup.transform);

                        Text nameText = newReward.transform.Find("name").GetComponent<Text>();
                        InventoryManager.instance.ObtainItem(treasure.Key,treasure.Value.Value<int>());
                        nameText.text = InventoryManager.instance.activeItems[treasure.Key].item.data.chineseName;
                        Image rewardImage = newReward.transform.Find("Image").GetComponent<Image>();
                        rewardImage.sprite = InventoryManager.instance.activeItems[treasure.Key].item.data.icon;
                        newReward.transform.Find("amount").GetComponent<Text>().text = treasure.Value.Value<int>().ToString();
                        
                    }
                }

                if (optionResult["success_bless"] != null)
                {
                    if (optionResult["success_bless"] is JObject blessObj)
                    {
                        
                        foreach (var prop in blessObj.Properties())
                        {
                            String blessText = "获得了新的祝福";

                            if (Blessings.ContainsKey(prop.Name))
                            {
                                Blessings[prop.Name] += prop.Value.Value<float>();
                            }
                            else
                            {
                                Blessings.Add(prop.Name, prop.Value.Value<float>());
                            }
                            
                            switch(prop.Name)
                            {
                                case "HP_max":
                                    foreach(CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                                    {
                                        character.additionalHealth += prop.Value.Value<float>() * character.health;
                                        character.health += character.additionalHealth;
                                        character.currentHealth = character.health;
                                    }
                                    break;
                                case "HP":
                                    foreach(CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                                    {
                                        blessText = "血量" + prop.Value.Value<float>() * 100 + "%";
                                        character.currentHealth += prop.Value.Value<float>() * character.health;
                                        if (character.currentHealth > character.health)
                                        {
                                            character.currentHealth = character.health;
                                        }
                                    }
                                    break;
                                case "Strength":
                                    foreach(CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                                    {
                                        character.strength += prop.Value.Value<float>();
                                        CharacterDetail.instance.UpdateCharacterDetails(character);
                                    }
                                    break;
                                case "Agility":
                                    foreach(CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                                    {
                                        character.agility += prop.Value.Value<float>();
                                        CharacterDetail.instance.UpdateCharacterDetails(character);
                                    }
                                    break;
                                case "Wisdom":
                                    foreach(CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                                    {
                                        character.intelligence += prop.Value.Value<float>();
                                        CharacterDetail.instance.UpdateCharacterDetails(character);
                                    }
                                    break;
                                case "SAN":
                                    blessText = "san值" + prop.Value.Value<float>() * 100 + "%";
                                    foreach (CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                                    {
                                        character.sanValue += prop.Value.Value<float>() * character.sanValue;
                                    }
                                    break;
                                case "SP":
                                    foreach (CharacterAttributes character in NewCharacterManager.instance.allCharacters)
                                    {
                                        character.sanValue += prop.Value.Value<float>() * character.sanValue;
                                    }
                                    break;
                                case "Dream_vision":
                                    break;
                                case "Cave_vision":
                                    break;
                                case "Garnett_vision":
                                    break;
                                    
                            }
                            StartCoroutine(FadeOutAndDeactivate(5f,blessText));

                        }
 
                    }
                }
                
                
                CharacterButton.SetActive(true);
                ToolButton.SetActive(true);
                BlessButton.SetActive(true);

                Time.timeScale = 1;
            });
            
        }
        else
        {
            Debug.LogWarning($"No valid data for {optionKey}");
        }
    }

    public void createInitialItem()
    {
        RewardPanel.SetActive(true);
                    
        foreach (Transform child in HorizontalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        
    
        GameObject Level_boost = Instantiate(RewardPrefab, HorizontalLayoutGroup.transform);
        GameObject Break_through = Instantiate(RewardPrefab, HorizontalLayoutGroup.transform);
        Text Level_boostText = Level_boost.transform.Find("name").GetComponent<Text>();
        InventoryManager.instance.ObtainItem("Level_boost",30);
        Level_boostText.text = InventoryManager.instance.activeItems["Level_boost"].item.data.chineseName;
        Image Level_boostImage = Level_boost.transform.Find("Image").GetComponent<Image>();
        Level_boostImage.sprite = InventoryManager.instance.activeItems["Level_boost"].item.data.icon;
        Level_boost.transform.Find("amount").GetComponent<Text>().text = 30.ToString();
        Text nameText = Break_through.transform.Find("name").GetComponent<Text>();
        InventoryManager.instance.ObtainItem("Break_through",6);
        nameText.text = InventoryManager.instance.activeItems["Break_through"].item.data.chineseName;
        Image rewardImage = Break_through.transform.Find("Image").GetComponent<Image>();
        rewardImage.sprite = InventoryManager.instance.activeItems["Break_through"].item.data.icon;
        Break_through.transform.Find("amount").GetComponent<Text>().text = 6.ToString();    

    }
    public IEnumerator FadeOutAndDeactivate(float duration, string text)
    {

        GameObject instance = Instantiate(BlessHint, HintLayout.transform);

        Image img = instance.GetComponent<Image>();
        if (img != null)
            img.color = new Color(1f, 1f, 1f, 1f);

        Text[] texts = instance.GetComponentsInChildren<Text>();
        foreach (Text t in texts)
        {
            t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);
            t.text = text;
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            if (img != null)
                img.color = new Color(1f, 1f, 1f, alpha);

            foreach (Text t in texts)
            {
                Color c = t.color;
                c.a = alpha;
                t.color = c;
            }
            yield return null;
        }

        Destroy(instance);
    }


}
