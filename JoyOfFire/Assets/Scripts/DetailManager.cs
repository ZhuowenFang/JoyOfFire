using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailManager : MonoBehaviour
{
    public Text TypeText;
    public Image CharacterImage;
    public Text CharacterName;
    public Text CharacterLevel;
    public Text CharacterRole;
    public Text CharacterHealth;
    public Image CharacterHealthBar;
    public Button CharacterAttributeDetailButton;
    public GameObject CharacterAttributeDetailPanel;
    public Text strengthText;
    public Text agilityText;
    public Text intelligenceText;
    public Text healthText;
    public Text physicalAttackText;
    public Text physicalDefenseText;
    public Text soulAttackText;
    public Text soulDefenseText;
    public Text speedText;
    public Text criticalRateText;
    public Text hitRateText;
    public Text tenacityRateText;
    public List<Text> skillNameTexts;
    public List<Text> skillDescriptionTexts;
    public List<Image> skillIcons;

    public GameObject buffLayoutGroup;
    public GameObject buffPrefab;
    public static DetailManager instance;
    
    public GameObject DetailPanel;
    public List<Button> characterButtons;
    public bool isEnemy = false;
    public Button switchButton;
    public Button backButton;
    
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        CharacterAttributeDetailButton.onClick.AddListener(() =>
        {
            CharacterAttributeDetailPanel.SetActive(!CharacterAttributeDetailPanel.activeSelf);
            
        });
        backButton.onClick.AddListener(() =>
        {
            DetailPanel.SetActive(false);
            Time.timeScale = 1;
        });
    }

   
    public void UpdateCharacterButtons()
    {
        if (!isEnemy)
        {
            switchButton.GetComponentInChildren<Text>().text = "敌人信息";
            foreach(Button button in characterButtons)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "角色" + (characterButtons.IndexOf(button) + 1).ToString();
            }
            for(int i = 0; i < BattleCharacterManager.instance.PlayerCharacters.Count; i++)
            {
                int index = i;
                characterButtons[i].interactable = true;
                characterButtons[i].onClick.RemoveAllListeners();
                characterButtons[i].onClick.AddListener(() =>
                {
                    ShowDetail(BattleCharacterManager.instance.PlayerCharacters[index], false);
                });
            }
        }
        else
        {
            switchButton.GetComponentInChildren<Text>().text = "我方信息";
            foreach(Button button in characterButtons)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "敌人" + (characterButtons.IndexOf(button) + 1).ToString();

            }
            for(int i = 0; i < BattleCharacterManager.instance.EnemyCharacters.Count; i++)
            {
                int index = i;
                characterButtons[i].interactable = true;
                characterButtons[i].onClick.RemoveAllListeners();
                characterButtons[i].onClick.AddListener(() =>
                {
                    ShowDetail(BattleCharacterManager.instance.EnemyCharacters[index], true);
                });
            }
        }
        switchButton.onClick.RemoveAllListeners();
        switchButton.onClick.AddListener(() =>
        {
            isEnemy = !isEnemy;
            UpdateCharacterButtons();
            if (isEnemy)
            {
                ShowDetail(BattleCharacterManager.instance.EnemyCharacters[0], true);
            }
            else
            {
                ShowDetail(BattleCharacterManager.instance.PlayerCharacters[0], false);
            }
        });
    }


    public void ShowDetail(ICharacter character, bool isEnemy)
    {
        Time.timeScale = 0;
        this.isEnemy = isEnemy;
        if (isEnemy)
        {
            TypeText.text = "敌人";
        }
        else
        {
            TypeText.text = "调查员";
        }
        DetailPanel.SetActive(true);
        
        if (character is CharacterAttributes cs)
        {
            StartCoroutine(ImageCache.GetTexture(cs.character_picture, (Texture2D texture) =>
            {
                if (texture != null)
                {
                    CharacterImage.sprite = Sprite.Create(texture, 
                        new Rect(0, 0, texture.width, texture.height), 
                        new Vector2(0.5f, 0.5f));
                }
            }));
            CharacterName.text = cs.characterName;
            CharacterLevel.text = "等级：" + cs.level.ToString();
            CharacterRole.text = "镜缘：" + cs.role;
            CharacterHealth.text = cs.currentHealth.ToString() + "/" + cs.health.ToString();
            CharacterHealthBar.fillAmount = (float)cs.currentHealth / (float)cs.health;
            strengthText.text = cs.strength.ToString();
            agilityText.text = cs.agility.ToString();
            intelligenceText.text = cs.intelligence.ToString();
            healthText.text = cs.health.ToString();
            physicalAttackText.text = cs.physicalAttack.ToString();
            physicalDefenseText.text = cs.physicalDefense.ToString();
            soulAttackText.text = cs.soulAttack.ToString();
            soulDefenseText.text = cs.soulDefense.ToString();
                
            speedText.text = cs.speed.ToString();
            criticalRateText.text = cs.criticalRate * 100 + "%";
            hitRateText.text = cs.hitRate * 100 + "%";
            tenacityRateText.text = cs.tenacityRate * 100 + "%";
            for (int i = 0; i < skillNameTexts.Count; i++)
            {
                skillNameTexts[i].text = "未获得";
                skillDescriptionTexts[i].text = "";
                skillIcons[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < cs.skills.Count; i++)
            {
                skillNameTexts[i].text = cs.skills[i].skillName;
                skillDescriptionTexts[i].text = cs.skills[i].skillDescription;
                skillIcons[i].gameObject.SetActive(true);
                StartCoroutine(ImageCache.GetTexture(cs.skills[i].skillIcon, (Texture2D texture) =>
                {
                    if (texture != null)
                    {
                        skillIcons[i].sprite = Sprite.Create(texture, 
                            new Rect(0, 0, texture.width, texture.height), 
                            new Vector2(0.5f, 0.5f));
                    }
                }));
                
            }
            foreach(Transform child in buffLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var buff in cs.activeBuffs)
            {
                GameObject buffObj = Instantiate(buffPrefab, buffLayoutGroup.transform);
                Text buffNameText = buffObj.transform.Find("Buffname").GetComponent<Text>();
                Text buffDescriptionText = buffObj.transform.Find("Buffdes").GetComponent<Text>();
                Text buffDurationText = buffObj.transform.Find("Bufflasttime").GetComponent<Text>();
                Image buffIcon = buffObj.transform.Find("Bufficon").GetComponent<Image>();
                buffNameText.text = buff.buffName;
                buffDurationText.text = "剩余" + buff.duration.ToString() + "回合";
                buffDescriptionText.text = buff.description;
                buffIcon.sprite = Resources.Load<Sprite>("buffIcons/" + buff.buffType);
            }
        } else if (character is MonsterAttributes ms)
        {
            CharacterImage.sprite = Resources.Load<Sprite>($"EnemyPics/{ms.monsterId}");
            CharacterName.text = ms.characterName;
            CharacterLevel.text = "等级：" + ms.level.ToString();
            CharacterRole.text = "";
            CharacterHealth.text = ms.currentHealth.ToString() + "/" + ms.health.ToString();
            CharacterHealthBar.fillAmount = (float)ms.currentHealth / (float)ms.health;
            strengthText.text = "未知";
            agilityText.text = "未知";
            intelligenceText.text = "未知";
            healthText.text = ms.health.ToString();
            physicalAttackText.text = ms.physicalAttack.ToString();
            physicalDefenseText.text = ms.physicalDefense.ToString();
            soulAttackText.text = ms.soulAttack.ToString();
            soulDefenseText.text = ms.soulDefense.ToString();
                
            speedText.text = ms.speed.ToString();
            criticalRateText.text = ms.criticalRate * 100 + "%";
            hitRateText.text = ms.hitRate * 100 + "%";
            tenacityRateText.text = ms.tenacityRate * 100 + "%";
            for (int i = 0; i < skillNameTexts.Count; i++)
            {
                skillNameTexts[i].text = "未获得";
                skillDescriptionTexts[i].text = "";
                skillIcons[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < ms.skills.Count; i++)
            {
                skillNameTexts[i].text = ms.skills[i].skillName;
                skillDescriptionTexts[i].text = ms.skills[i].skillDescription;
                skillIcons[i].gameObject.SetActive(false);
                
            }
            foreach(Transform child in buffLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var buff in ms.activeBuffs)
            {
                GameObject buffObj = Instantiate(buffPrefab, buffLayoutGroup.transform);
                Text buffNameText = buffObj.transform.Find("Buffname").GetComponent<Text>();
                Text buffDescriptionText = buffObj.transform.Find("Buffdes").GetComponent<Text>();
                Text buffDurationText = buffObj.transform.Find("Bufflasttime").GetComponent<Text>();
                Image buffIcon = buffObj.transform.Find("Bufficon").GetComponent<Image>();
                buffNameText.text = buff.buffName;
                buffDurationText.text = "剩余" + buff.duration.ToString() + "回合";
                buffDescriptionText.text = buff.description;
                buffIcon.sprite = Resources.Load<Sprite>("buffIcons/" + buff.buffType);
            }
        }
        
    }
}
