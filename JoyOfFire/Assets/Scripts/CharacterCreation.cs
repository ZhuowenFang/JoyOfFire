using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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

    private void OnCreateButtonClicked()
    {
        string profession = professionInput.text;
        string clothes = clothesInput.text;
        string combat = combatInput.text;
        string other = otherInput.text;

        if (string.IsNullOrEmpty(selectedGender) ||
            string.IsNullOrEmpty(clothes) || string.IsNullOrEmpty(combat) || string.IsNullOrEmpty(other) || string.IsNullOrEmpty(profession))
        {
            Debug.LogError("请填写所有字段！");
            return;
        }
        if (ContainsForbiddenWord(profession) || ContainsForbiddenWord(clothes) || ContainsForbiddenWord(combat) || ContainsForbiddenWord(other))
        {
            
            Debug.LogError("输入内容包含违禁词，请重新输入！");
            return;
        }
        ClassManager.CharacterCreationData characterCreationData = new ClassManager.CharacterCreationData
        {
            sex = selectedGender,
            profession = profession,
            clothes = clothes,
            combat = combat,
            other = other
        };
        
        string jsonData = JsonUtility.ToJson(characterCreationData);

        Debug.Log($"角色数据: {jsonData}");
        APIManager.instance.CreateCharacter(
            jsonData,
            onSuccess: (response) =>
            {
                WaitingPanel.SetActive(false);
                Debug.Log($"Character Created: {response}");
                var characterResponse = JsonConvert.DeserializeObject<CharacterAttributesResponse>(response);
                var character = NewCharacterManager.ConvertToCharacterAttributes(characterResponse.data);
                NewCharacterManager.instance.AddCharacter(character);
                NewCharacterManager.instance.Role.text = selectedProfession;
            },
            onError: (error) =>
            {
                Debug.LogError($"Error creating character: {error}");
            }
        );
        
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
