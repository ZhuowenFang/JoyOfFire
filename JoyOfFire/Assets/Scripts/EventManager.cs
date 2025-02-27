using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
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
    private List<string> levelPool;
    public ClassManager.LevelResponse levelResponse;
    public GameObject CharacterPanel;
    public Button ConfirmButton;
    private string APIresponse;
    public Dictionary<string, JObject> optionResults = new Dictionary<string, JObject>();
    public GameObject RewardPanel;
    public GameObject RewardPrefab;
    public GameObject HorizontalLayoutGroup;
    public string levelOrder = "1";
    void InitializeLevelPool()
    {
        levelPool = new List<string>();

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
    }

    void Start()
    {
        foreach (var hex in FindObjectsOfType<HexagonEvent>())
        {
            hexagons.Add(hex);
        }
        InitializeLevelPool();
    }

    void Update()
    {
        CheckPlayerPosition();
    }

    void CheckPlayerPosition()
    {
        Vector3 playerPosition = player.transform.position;

        foreach (var hex in hexagons)
        {
            float distance = Vector3.Distance(playerPosition, hex.center);

            if (distance <= hex.radius)
            {
                TriggerHexEvent(hex);
                break;
            }
        }
    }

    void TriggerHexEvent(HexagonEvent hex)
    {
        if (string.IsNullOrEmpty(hex.eventNumber))
        {
            int randomIndex = UnityEngine.Random.Range(0, levelPool.Count);
            level = levelPool[randomIndex];
        }
        else
        {
            level = hex.eventStage + "-" + hex.eventNumber;
            levelOrder = hex.eventStage;
        }
        APIManager.instance.GetLevelData(
            level,
            onSuccess: (response) =>
            {
                Time.timeScale = 0;
                Debug.Log($"成功响应：{response}");
                levelResponse = JsonUtility.FromJson<ClassManager.LevelResponse>(response);
                CharacterPanel.SetActive(true);
                CharacterButton.SetActive(false);
                CharacterSelectionEventTitle.text = level + " " + levelResponse.data.levelInfo.level_name;

                ConfirmButton.onClick.AddListener(() =>
                {
                    ParseLevelData();
                    
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

    public void ParseLevelData()
    {
        Debug.Log($"Level Name: {levelResponse.data.levelInfo.level_name}");
        Debug.Log($"Level Description: {levelResponse.data.levelInfo.level_desc}");
        Debug.Log($"Option A Text: {levelResponse.data.levelInfo.option_a_experience}");
        EventPanel.SetActive(true);
        EventTitle.text = level + " " + levelResponse.data.levelInfo.level_name;
        EventDescription.text = levelResponse.data.levelInfo.level_desc;

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

                Debug.Log($"{optionKey} Rewards: {optionResult["rewards"]}");
                Debug.Log($"{optionKey} Copywriting: {optionResult["success_copywriting"]}");
                Debug.Log($"{optionKey} Result: {optionResult["next_stage"]}");
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
        if (string.IsNullOrEmpty(optionText) || optionText == "nan" || !optionResults.ContainsKey(optionKey))
        {
            choiceButton.gameObject.SetActive(false);
        }
        else
        {
            choiceButton.gameObject.SetActive(true);
            choiceButton.GetComponentInChildren<Text>().text = optionText;

            choiceButton.onClick.RemoveAllListeners();
            choiceButton.onClick.AddListener(() => HandleChoice(optionKey,choiceButton));
        }
    }

    private void HandleChoice(string optionKey,Button choiceButton)
    {
        if (optionResults.TryGetValue(optionKey, out JObject optionResult) && optionResult != null)
        {
            Debug.Log($"Handling choice for {optionKey}");
            Debug.Log($"Tool Required: {optionResult["tool_required"]}");
            Debug.Log($"Success Condition: {optionResult["success_condition"]}");
            Debug.Log($"Mini Game: {optionResult["mini_game"]}");
            Debug.Log($"Monsters: {optionResult["monsters"]}");
            if (optionResult["monsters"] != null)
            {
                if (optionResult["monsters"] is JObject monsters)
                {
                    foreach (var monster in monsters)
                    {
                        Debug.Log($"Monster: {monster.Key}, Count: {monster.Value}");
                    }
                }
                else
                {
                    Debug.LogWarning("Monsters is not a valid JObject.");
                }
            }
            Debug.Log($"Rewards: {optionResult["rewards"]}");
            if (optionResult["rewards"] != null)
            {
                if (optionResult["rewards"] is JObject rewards)
                {
                    foreach (var reward in rewards)
                    {
                        Debug.Log($"Reward: {reward.Key}, Amount: {reward.Value}");
                    }
                }
                else
                {
                    Debug.LogWarning("Rewards is not a valid JObject.");
                }
            }
            Debug.Log($"Copywriting: {optionResult["success_copywriting"]}");
            Debug.Log($"Result: {optionResult["next_stage"]}");
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
                EventPanel.SetActive(false);
                ChoiceDetail.gameObject.SetActive(false);
                if (optionResult["monsters"].Count() != 0 && optionResult["monsters"] is JObject monsters)
                {
                    
                    int totalAPICalls = monsters.Count; // 总共需要调用的怪物 API 数量
                    int finishedAPICalls = 0;            // 已完成的调用数量

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
                                // 检查是否所有API调用都完成了
                                if (finishedAPICalls >= totalAPICalls)
                                {
                                    SceneManager.LoadScene("Battle", LoadSceneMode.Additive);
                                }
                            },
                            (error) =>
                            {
                                Debug.LogError($"加载怪物失败: {error}");
                            });
                        
                    }
                    SceneManager.LoadScene("Battle", LoadSceneMode.Additive);

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
                        
                    }
                }
                CharacterButton.SetActive(true);

                Time.timeScale = 1;
            });
            
        }
        else
        {
            Debug.LogWarning($"No valid data for {optionKey}");
        }
    }
}
