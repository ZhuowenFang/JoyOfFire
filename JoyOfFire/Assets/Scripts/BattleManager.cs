using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public SpeedBarUI speedBarUI;
    public RectTransform actionPanel;
    public Button attackButton; 
    public Button endTurnButton;
    public List<Button> characterButtons;
    public List<Button> enemyButtons;

    private bool isSelectingEnemy = false;
    private float animationTimer = 0f;
    public static BattleManager instance;
    
    public float criticalMultiplier = 1.5f; 
    public GameObject damageTextPrefab;
    public Canvas mainCanvas;  
    public bool isUsingSkill = false;
    private Dictionary<int, float> textOffsets = new Dictionary<int, float>();
    public TMP_Text energyText;
    
    public GameObject skillButtonPrefab;
    public Transform skillButtonParent;
    private List<Button> skillButtons = new List<Button>();
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (var button in CharacterManager.instance.characterButtons)
        {
            characterButtons.Add(button);
        }

        foreach (var button in CharacterManager.instance.EnemyButtons)
        {
            enemyButtons.Add(button);
        }
        foreach (var button in characterButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    
        for (int i = 0; i < enemyButtons.Count; i++)
        {
            int index = i;
            enemyButtons[i].onClick.AddListener(() => OnEnemySelected(index));
        }

        attackButton.onClick.AddListener(() =>
        {
            isUsingSkill = false;
            StartEnemySelection();
        });
        

        // skillButton.onClick.AddListener(() =>
        // {
        //     isUsingSkill = true;
        //     StartEnemySelection();
        // });
        endTurnButton.onClick.AddListener(EndTurn);
        StartNextTurn();
    }

    public async void StartNextTurn()
    {
        actionPanel.gameObject.SetActive(false);
        ResetAllCharacterSizes();
        
        ICharacter nextCharacter = speedBarUI.GetNextCharacter();
        if (nextCharacter == null)
        {
            Debug.Log("没有角色进行回合！");
            return;
        }

        Debug.Log($"{nextCharacter.index} 开始行动！");

        int characterIndex = nextCharacter.index;

        if (characterIndex >= CharacterManager.instance.PlayerCharacters.Count)
        {
            actionPanel.gameObject.SetActive(false);
            characterButtons[characterIndex].transform.localScale = Vector3.one * 1.2f;
            await Task.Delay(2000);
            BaseEnemy enemyScript = characterButtons[characterIndex].GetComponent<BaseEnemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeTurn();
            }
            else
            {
                Debug.LogWarning($"Enemy {characterIndex} 没有 BaseEnemy 组件！");
                EndTurn();
            }            
            Debug.Log($"敌人 {characterIndex} 行动完成！");

        }
        else
        {
            actionPanel.gameObject.SetActive(true);
            characterButtons[characterIndex].transform.localScale = Vector3.one * 1.2f;
            energyText.text = $"{nextCharacter.energy} / {nextCharacter.maxEnergy}";

            GenerateSkillButtons(nextCharacter as CharacterAttributes);
            
        }
    }

    private void GenerateSkillButtons(CharacterAttributes character)
    {
        foreach (var btn in skillButtons)
        {
            Destroy(btn.gameObject);
        }
        skillButtons.Clear();

        foreach (var skill in character.skills)
        {
            
            if (skill.skillName == "") continue;
            GameObject skillButtonGO = Instantiate(skillButtonPrefab, skillButtonParent);
            Button skillButton = skillButtonGO.GetComponent<Button>();
            Text skillText = skillButtonGO.GetComponentInChildren<Text>();

            skillText.text = $"{skill.skillName} (消耗: {skill.skillCost})";

            skillButton.interactable = character.energy >= skill.skillCost;

            skillButton.onClick.AddListener(() => OnSkillSelected(skillButton,character, skill));

            skillButtons.Add(skillButton);
        }
    }
    
    private void OnSkillSelected(Button skillButton, CharacterAttributes attacker, SkillAttributes skill)
    {
        isUsingSkill = true;
        actionPanel.gameObject.SetActive(false);
    
        isSelectingEnemy = true;

        skillButton.onClick.RemoveAllListeners();
        skillButton.onClick.AddListener(() =>
        {
            if (isUsingSkill)
            {
                ICharacter defender = CharacterManager.instance.EnemyCharacters[0]; // 这里你需要让玩家选择敌人
                UseSkill(attacker, defender, skill);
                attacker.energy -= skill.skillCost;
                isSelectingEnemy = false;
                ResetAllCharacterSizes();
                EndTurn();
            }
        });
    }


    public void StartEnemySelection()
    {
        isSelectingEnemy = true;
        actionPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isSelectingEnemy)
        {
            AnimateEnemySelection();
        }
    }

    private void AnimateEnemySelection()
    {
        animationTimer += Time.deltaTime * 2f;
        float scale = 1f + Mathf.Sin(animationTimer) * 0.2f;

        foreach (var enemy in enemyButtons)
        {
            enemy.transform.localScale = Vector3.one * scale;
        }
    }

    public void OnEnemySelected(int enemyIndex)
    {
        if (!isSelectingEnemy) return;

        ICharacter attacker = SpeedBarUI.instance.GetNextCharacter();
        ICharacter defender = CharacterManager.instance.EnemyCharacters[enemyIndex];

        // if (isUsingSkill)
        // {
        //     UseSkill(attacker, defender, attacker.skillAttributes);
        //     attacker.energy -= attacker.skillAttributes.cost;
        // }
        // else
        // {
            DealDamage(attacker, defender, 1f);
            if (attacker.energy < attacker.maxEnergy)
            {
                attacker.energy += 1;
            }
        // }

        isSelectingEnemy = false;
        ResetAllCharacterSizes();
        EndTurn();
    }
    
    public void DealDamage(ICharacter attacker, ICharacter defender, float damageMultiplier)
    {
        float levelDifference = Mathf.Abs(attacker.level - defender.level);
        float damageReductionPercentage = defender.physicalDefense / 
                                          (defender.physicalDefense + levelDifference * attacker.damageX1 + attacker.damageX2);

        float damage = attacker.physicalAttack * damageMultiplier * (1 - damageReductionPercentage);

        float randomValue = Random.value;
        bool isCritical = randomValue <= attacker.criticalRate;
        if (isCritical)
        {
            damage *= criticalMultiplier;
            Debug.Log($"暴击！造成了 {damage} 点伤害！");
        }
        else
        {
            Debug.Log($"普通攻击造成了 {damage} 点伤害！");
        }

        ApplyDamage(defender, damage, isCritical);
    }

    public void ApplyDamage(ICharacter defender, float damage, bool isCritical, bool isEnemy = false)
    {
        float remainingShield = Mathf.Max(defender.shieldAmount - damage, 0);
        float damageToHealth = Mathf.Max(damage - defender.shieldAmount, 0);
        defender.shieldAmount = remainingShield;
        defender.currentHealth -= damageToHealth;
        Debug.Log($"{defender.characterName} 受到了 {damageToHealth} 点伤害！");
        Debug.Log($"敌人现在的生命值：{defender.currentHealth}");
        ShowDamageText(defender.index, damage, isCritical, isEnemy);

        if (defender.currentHealth <= 0)
        {
            Debug.Log($"{defender.characterName} 被击败！");
            // RemoveCharacter(defender);
        }
    }
    
    public void UseSkill(ICharacter attacker, ICharacter defender, SkillAttributes skill)
    {
        float levelDifference = Mathf.Abs(attacker.level - defender.level);

        // 计算物理伤害
        float physicalDamageReduction = defender.physicalDefense /
            (defender.physicalDefense + levelDifference * attacker.damageX1 + attacker.damageX2);
        float physicalDamage = attacker.physicalAttack * skill.physicalDamage * (1 - physicalDamageReduction);

        // 计算灵魂伤害
        float soulDamageReduction = defender.soulDefense /
            (defender.soulDefense + levelDifference * attacker.damageX1 + attacker.damageX2);
        float soulDamage = attacker.soulAttack * skill.soulDamage * (1 - soulDamageReduction);

        // 总伤害 = 物理伤害 + 灵魂伤害
        float totalDamage = physicalDamage + soulDamage;

        // 暴击判定
        float effectiveCriticalRate = attacker.criticalRate + skill.criticalBoost;
        bool isCritical = Random.value < effectiveCriticalRate;
        if (isCritical)
        {
            totalDamage *= 1.5f;
            Debug.Log($"暴击！造成了 {totalDamage:F0} 点伤害！");
        }
        else
        {
            Debug.Log($"普通技能造成了 {totalDamage:F0} 点伤害！");
        }

        // 应用伤害
        ApplyDamage(defender, totalDamage,isCritical);

        // 技能附加效果
        ApplySkillEffects(attacker, defender, skill);
    }

    private void ApplySkillEffects(ICharacter attacker, ICharacter defender, SkillAttributes skill)
    {
        // 状态效果应用
        if (skill.stunChance > 0 && Random.value < skill.stunChance)
        {
            ShowText(defender.index, "眩晕!",Color.yellow);
            Debug.Log($"{defender.characterName} 被眩晕！");
        }

        if (skill.silenceChance > 0 && Random.value < skill.silenceChance)
        {
            ShowText(defender.index, "沉默!",Color.blue);
            Debug.Log($"{defender.characterName} 被沉默！");
        }

        if (skill.blockChance > 0 && Random.value < skill.blockChance)
        {
            attacker.tenacityRate += 0.3f;
            attacker.physicalDefense *= 1.2f;
            attacker.soulDefense *= 1.2f;

            ShowText(attacker.index, "变得更肉了!",Color.green);
            Debug.Log($"{attacker.characterName} 变得更肉了！");
        }

        if (skill.shieldAmount > 0)
        {
            attacker.shieldAmount += skill.shieldAmount;

            ShowText(attacker.index, $"护盾 +{skill.shieldAmount}",Color.green);
            Debug.Log($"{attacker.characterName} 获得了 {skill.shieldAmount} 点护盾！");
        }

        if (skill.healAmount > 0)
        {
            float healedAmount = Mathf.Min(skill.healAmount, attacker.health - attacker.currentHealth);
            attacker.currentHealth = Mathf.Min(attacker.currentHealth + skill.healAmount, attacker.health);

            ShowText(attacker.index, $"治疗 +{healedAmount:F0}",Color.green);
            Debug.Log($"{attacker.characterName} 回复了 {healedAmount} 点生命值！");
        }
    }



    private void ShowDamageText(int characterIndex, float damage, bool isCritical, bool isEnemy = false)
    {
        Debug.Log($"显示伤害文本：{characterIndex}");
        RectTransform targetRect = CharacterManager.instance.characterButtons[characterIndex].GetComponent<RectTransform>();

        GameObject damageText = Instantiate(damageTextPrefab, mainCanvas.transform);
        if (isCritical)
        {
            damageText.GetComponent<TMP_Text>().text = $"暴击！-{damage:F0}";
        }
        else
        {
            damageText.GetComponent<TMP_Text>().text = $"-{damage:F0}";

        }

        RectTransform textRect = damageText.GetComponent<RectTransform>();
        textRect.SetParent(mainCanvas.transform, false);
        textRect.localPosition = targetRect.localPosition + new Vector3(0, 300, 0);

        damageText.transform.SetAsLastSibling();

        
        Destroy(damageText, 1f);
    }
    

    public void ShowText(int characterIndex, string text,Color color)
    {
        RectTransform targetRect = CharacterManager.instance.characterButtons[characterIndex].GetComponent<RectTransform>();

        GameObject statusText = Instantiate(damageTextPrefab, mainCanvas.transform);
        statusText.GetComponent<TMP_Text>().text = text;
        statusText.GetComponent<TMP_Text>().color = color;

        if (!textOffsets.ContainsKey(characterIndex))
        {
            if (characterIndex >= CharacterManager.instance.PlayerCharacters.Count)
            {
                textOffsets[characterIndex] = -300f;
            }
            else
            {
                textOffsets[characterIndex] = -350f;

            }
            
        }

        RectTransform textRect = statusText.GetComponent<RectTransform>();
        textRect.SetParent(mainCanvas.transform, false);
        textRect.localPosition = targetRect.localPosition + new Vector3(0, textOffsets[characterIndex], 0);
        statusText.transform.SetAsLastSibling();

        textOffsets[characterIndex] -= 100f;

        StartCoroutine(ResetTextOffset(characterIndex, statusText));
    }

    private IEnumerator ResetTextOffset(int characterIndex, GameObject textObject)
    {
        yield return new WaitForSeconds(1.5f);

        // 销毁文本对象
        Destroy(textObject);

        // 恢复偏移量
        if (textOffsets.ContainsKey(characterIndex))
        {
            textOffsets[characterIndex] += 100f;
        }
    }



    private void ResetAllCharacterSizes()
    {
        foreach (var button in characterButtons)
        {
            button.transform.localScale = Vector3.one;  // 重置图片大小
        }
    }

    public void EndTurn()
    {
        Debug.Log("回合结束！");
        ResetAllCharacterSizes();
        speedBarUI.CompleteCurrentTurn();
        StartNextTurn();
    }
    public void EndBattle()
    {
        SceneManager.UnloadSceneAsync("Battle");
    }
}
