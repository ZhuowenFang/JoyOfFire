using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;

public class CharacterCreation : MonoBehaviour
{
    [Header("职业选择页面")]
    public GameObject professionPage; // 职业选择页面
    public List<Button> professionButtons; // 职业按钮列表
    public Button nextButton; // 下一步按钮

    [Header("角色信息页面")]
    public GameObject characterInfoPage; // 角色信息页面
    public List<Button> genderButtons;
    public InputField professionInput; // 职业输入框
    public InputField clothesInput; // 衣服输入框
    public InputField combatInput; // 战斗细节输入框
    public InputField otherInput; // 其他输入框
    public Button createButton; // 创建角色按钮

    private string selectedProfession; // 选中的职业
    private string selectedGender; // 选中的性别
    private HashSet<string> forbiddenWords; // 用于存储违禁词
    public GameObject WaitingPanel;
    
    public GameObject IlegalWordPanel;
    
    public GameObject characterFeaturePanel;

    public Button backButton;
    public int currentIndex = 0;
    
    public static CharacterCreation instance;
    private void Awake()
    {
        instance = this;
    }
    
    
    void Start()
    {
        LoadForbiddenWords(); // 加载违禁词

        nextButton.interactable = false;
        selectedProfession = null;
        selectedGender = null;

        foreach (Button button in professionButtons)
        {
            button.onClick.AddListener(() => OnProfessionSelected(button));
        }

        foreach (Button button in genderButtons)
        {
            button.onClick.AddListener(() => OnGenderSelected(button));
        }

        nextButton.onClick.AddListener(OnNextButtonClicked);

        createButton.onClick.AddListener(OnCreateButtonClicked);
        // createInitialCharacter();
    }



    private void OnProfessionSelected(Button button)
    {
        // 查找子对象 RoleName 并获取 Text 组件
        Transform roleNameTransform = button.transform.Find("RoleName");
        if (roleNameTransform != null)
        {
            Text roleNameText = roleNameTransform.GetComponent<Text>();
            if (roleNameText != null)
            {
                selectedProfession = roleNameText.text; // 获取职业文字
                Debug.Log($"Selected Profession: {selectedProfession}");
                nextButton.interactable = true; // 激活下一步按钮
            }
            else
            {
                Debug.LogError("RoleName 对象未找到 Text 组件！");
            }
        }
        else
        {
            Debug.LogError("未找到名为 RoleName 的子对象！");
        }
    }


    private void LoadForbiddenWords()
    {
        forbiddenWords = new HashSet<string>();
        string filePath = Application.streamingAssetsPath + "/sensitive_stop_words.txt";
    
        Debug.Log($"违禁词文件路径: {filePath}");

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    forbiddenWords.Add(trimmedLine);
                }
            }
            Debug.Log($"加载完成，共加载 {forbiddenWords.Count} 个违禁词");
        }
        else
        {
            Debug.LogError("违禁词文件未找到！");
        }
    }

    private bool ContainsForbiddenWord(string input)
    {
        foreach (var word in forbiddenWords)
        {
            if (input.Contains(word))
            {
                return true;
            }
        }
        return false;
    }

    private void OnGenderSelected(Button button)
    {
        selectedGender = button.GetComponentInChildren<Text>().text; // 获取性别文字

        // foreach (Button btn in genderButtons)
        // {
        //     ColorBlock colors = btn.colors;
        //     colors.normalColor = (btn == button) ? Color.green : Color.white; // 已选性别按钮高亮
        //     btn.colors = colors;
        // }
    }

    private void OnNextButtonClicked()
    {
        professionPage.SetActive(false);
        characterInfoPage.SetActive(true);
    }

    private async void OnCreateButtonClicked()
    {
        
        string profession = professionInput.text;
        string clothes = clothesInput.text;
        string combat = combatInput.text;
        string other = otherInput.text;

        // if (string.IsNullOrEmpty(selectedGender) ||
        //     string.IsNullOrEmpty(clothes) || string.IsNullOrEmpty(combat) || string.IsNullOrEmpty(other) || string.IsNullOrEmpty(profession))
        // {
        //     Debug.LogError("请填写所有字段！");
        //     return;
        // }
        if (ContainsForbiddenWord(profession) || ContainsForbiddenWord(clothes) || ContainsForbiddenWord(combat) || ContainsForbiddenWord(other))
        {
            
            Debug.LogError("输入内容包含违禁词，请重新输入！");
            IlegalWordPanel.SetActive(true);
            ClearInputs();
            return;
        }
        characterFeaturePanel.SetActive(false);
        WaitingPanel.SetActive(true);
        WaitingPanel.transform.Find("Text (Legacy)").GetComponent<TMP_Text>().text = "创建中...";
        // backButton.interactable = true;
        ClassManager.CharacterCreationData characterCreationData = new ClassManager.CharacterCreationData
        {
            userId = "1",
            sex = selectedGender,
            profession = profession,
            clothes = clothes,
            combat = combat,
            other = other
        };
        
        APIManager.instance.GetLevelData(
            "1-1",
            onSuccess: (response) =>
            {
                
            },
            onError: (error) =>
            {
                Debug.LogError($"Error getting level data: {error}");
            }
        );
        
        string jsonData = JsonUtility.ToJson(characterCreationData);
        NewCharacterManager.instance.isCharacterCreating[currentIndex] = true;
        Debug.Log($"角色数据: {jsonData}");
        APIManager.instance.CreateCharacter(
            jsonData,
            onSuccess: async (response) =>
            {
                Debug.Log($"Character Created: {response}");
                var characterResponse = JsonConvert.DeserializeObject<CharacterAttributesResponse>(response);
                if (characterResponse.code != "00000")
                {
                    EventManager.instance.StartCoroutine(EventManager.instance.FadeOutAndDeactivate(5f,"角色创建失败"));
             
                    Debug.LogError($"Error creating character: {characterResponse.message}");
                    NewCharacterManager.instance.isCharacterCreating[currentIndex] = false;
                    WaitingPanel.transform.Find("Text (Legacy)").GetComponent<TMP_Text>().text = "创建失败,请稍后再试";
                    await Task.Delay(3000);
                    NewCharacterManager.instance.InitializeButtons();
                    WaitingPanel.SetActive(false);
                    return;
                }
                EventManager.instance.StartCoroutine(EventManager.instance.FadeOutAndDeactivate(5f,"角色已创建完成"));
        
                WaitingPanel.SetActive(false);
        
                var character = NewCharacterManager.ConvertToCharacterAttributes(characterResponse.data);
                NewCharacterManager.instance.AddCharacter(character);
                character.role = selectedProfession;
                CharacterDetail.instance.Role.text = selectedProfession;
                NewCharacterManager.instance.isCharacterCreating[currentIndex] = false;
                NewCharacterManager.instance.InitializeButtons();
        
            },
            onError: async (error) =>
            {                
                EventManager.instance.StartCoroutine(EventManager.instance.FadeOutAndDeactivate(5f,"角色创建失败"));
        
                Debug.LogError($"Error creating character: {error}");
                NewCharacterManager.instance.isCharacterCreating[currentIndex] = false;
                WaitingPanel.transform.Find("Text (Legacy)").GetComponent<TMP_Text>().text = "创建失败,请稍后再试";
                await Task.Delay(3000);
                NewCharacterManager.instance.InitializeButtons();
                WaitingPanel.SetActive(false);
            }
        );
        // string mockResponse = @"{
        //     ""basic_information"": {
        //         ""appearance"": ""身穿白大褂，手持听诊器，周围狂风呼啸"",
        //         ""fighting_ability"": ""狂风之力，可辅助可攻击"",
        //         ""gender"": ""男"",
        //         ""name"": ""狂风医生"",
        //         ""story"": ""拥有狂风之力的医生，在克苏鲁世界救死扶伤""
        //     },
        //     ""character_picture"": ""https://s.coze.cn/t/CnQK0oHlBLj415s2/"",
        //     ""current_ability"": [""- 智力：3"", ""- 力量：6"", ""- 敏捷：1""],
        //     ""potential_ability"": [""- 智力：25"", ""- 力量：23"", ""- 敏捷：15""],
        //     ""talent1"": {
        //         ""abilitydescription"": ""以风之力进行诊疗，造成 148.14% 的物理伤害，同时给自己增加 51.04 的护盾。"",
        //         ""cost"": ""2"",
        //         ""description"": [{
        //             ""talent_description"": ""以风之力治疗与攻击"",
        //             ""talent_name"": ""风暴诊疗""
        //         }],
        //         ""icon"": ""https://s.coze.cn/t/ClmrB2ygjB0IqGk8/""
        //     },
        //     ""talent_count1"": [""1.4814"", ""0.0000"", ""1"", ""1"", ""1"", ""51.0398"", ""0.0000"", ""0.0000""],
        //     ""talent2"": null,
        //     ""talent_count2"": null,
        //     ""talent3"": null,
        //     ""talent_count3"": null,
        //     ""experience"": null,
        // }";
        //
        // var characterResponse = JsonConvert.DeserializeObject<ClassManager.CharacterData>(mockResponse);
        //
        // var character = NewCharacterManager.ConvertToCharacterAttributes(characterResponse);
        // character.user_id = "1";
        //
        // character.id = "1";
        // character.character_id = "10";
        // NewCharacterManager.instance.AddCharacter(character);
        //
        // CharacterDetail.instance.Role.text = selectedProfession;
        // character.role = selectedProfession;
        //
        
        

        
        ClearInputs();
    }

    private void ClearInputs()
    {
        professionInput.text = "";
        clothesInput.text = "";
        combatInput.text = "";
        otherInput.text = "";

        foreach (Button btn in genderButtons)
        {
            ColorBlock colors = btn.colors;
            colors.normalColor = Color.white;
            btn.colors = colors;
        }

        selectedGender = null;
    }
    [System.Serializable]
    public class CharacterAttributesResponse
    {
        public string code;
        public string message;
        public ClassManager.CharacterData data;
    }
}
