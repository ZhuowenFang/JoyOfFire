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
    
    private SkillAttributes selectedSkill;
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
        selectedSkill = skill;
        Debug.Log(skill.physicalDamage);
        StartEnemySelection();
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
        Debug.Log($"选择了敌人 {enemyIndex}！");

        ICharacter attacker = SpeedBarUI.instance.GetNextCharacter();
        ICharacter defender = CharacterManager.instance.EnemyCharacters[enemyIndex];

        if (isUsingSkill)
        {
            UseSkill(attacker, defender, selectedSkill);
            attacker.energy -= selectedSkill.skillCost;
        }
        else
        {
            DealDamage(attacker, defender, 1f);
            if (attacker.energy < attacker.maxEnergy)
            {
                attacker.energy += 1;
            }
        }

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
        ShowDamageText(defender.index, damageToHealth, isCritical, isEnemy);

        if (defender.currentHealth <= 0)
        {
            Debug.Log($"{defender.characterName} 被击败！");
            RemoveCharacterFromBattle(defender);
            CheckBattleOutcome();
        }
    }
    
    public void UseSkill(ICharacter attacker, ICharacter defender, SkillAttributes skill)
    {
        float levelDifference = Mathf.Abs(attacker.level - defender.level);

        // 计算物理伤害
        float physicalDamageReduction = defender.physicalDefense /
            (defender.physicalDefense + levelDifference * attacker.damageX1 + attacker.damageX2);
        Debug.Log(attacker.physicalAttack);
        Debug.Log(skill.physicalDamage);
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
        // **眩晕**
        if (skill.stunChance > 0 && Random.value < skill.stunChance)
        {
            defender.activeBuffs.Add(new BuffEffect("眩晕", BuffType.Stun, 1, 0, true));
            ShowText(defender.index, "眩晕!", Color.yellow);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 眩晕！");
        }

        // **沉默**
        if (skill.silenceChance > 0 && Random.value < skill.silenceChance)
        {
            defender.activeBuffs.Add(new BuffEffect("沉默", BuffType.Silence, 1, 0, true));
            ShowText(defender.index, "沉默!", Color.blue);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 沉默！");
        }

        // **增加护盾**
        if (skill.shieldAmount > 0)
        {
            attacker.shieldAmount += skill.shieldAmount;
            ShowText(attacker.index, $"护盾 +{skill.shieldAmount}", Color.green);
            Debug.Log($"{attacker.characterName} 获得 {skill.shieldAmount} 点护盾！");
        }

        // **治疗**
        if (skill.healAmount > 0)
        {
            float healedAmount = Mathf.Min(skill.healAmount, attacker.health - attacker.currentHealth);
            attacker.currentHealth = Mathf.Min(attacker.currentHealth + skill.healAmount, attacker.health);
            ShowText(attacker.index, $"治疗 +{healedAmount:F0}", Color.green);
            Debug.Log($"{attacker.characterName} 回复 {healedAmount} 点生命值！");
        }

        // **格挡**
        if (skill.blockChance > 0 && Random.value < skill.blockChance)
        {
            attacker.activeBuffs.Add(new BuffEffect("格挡", BuffType.DefenseBoost, 2, 0.2f, false));
            ShowText(attacker.index, "格挡!", Color.gray);
            Debug.Log($"{attacker.characterName} 进入格挡状态，伤害减少 20%！");
        }
        
    }



    private void ShowDamageText(int characterIndex, float damage, bool isCritical, bool isEnemy = false)
    {
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

        Destroy(textObject);

        if (textOffsets.ContainsKey(characterIndex))
        {
            textOffsets[characterIndex] += 100f;
        }
    }



    private void ResetAllCharacterSizes()
    {
        foreach (var button in characterButtons)
        {
            button.transform.localScale = Vector3.one;
        }
    }
    private void RemoveCharacterFromBattle(ICharacter deadCharacter)
    {
        // 根据 deadCharacter 类型判断是玩家还是敌人
        if (deadCharacter is CharacterAttributes)
        {
            // 玩家角色死亡
            CharacterAttributes deadPlayer = deadCharacter as CharacterAttributes;
            // 从玩家列表中移除
            CharacterManager.instance.PlayerCharacters.Remove(deadPlayer);
            // 从所有角色按钮列表中移除并销毁对应的UI
            RemoveCharacterButton(deadPlayer.index, false);
        }
        else if (deadCharacter is MonsterAttributes)
        {
            // 敌人角色死亡
            MonsterAttributes deadMonster = deadCharacter as MonsterAttributes;
            CharacterManager.instance.EnemyCharacters.Remove(deadMonster);
            RemoveCharacterButton(deadMonster.index, true);
        }
    
        // 同时从 SpeedBarUI 中删除该角色（假设 SpeedBarUI 有类似 RemoveCharacter(int index) 方法）
        speedBarUI.RemoveCharacter(deadCharacter);
    }

    /// <summary>
    /// 根据索引移除角色按钮（销毁对应UI对象，并从列表中移除）。
    /// </summary>
    private void RemoveCharacterButton(int index, bool isEnemy)
    {
        if (index >= 0 && index < CharacterManager.instance.characterButtons.Count)
        {
            
            Button btn = CharacterManager.instance.characterButtons[index];
            GameObject btnGO = btn.gameObject;
            Destroy(btnGO);
            CharacterManager.instance.characterButtons.RemoveAt(index);
            characterButtons.RemoveAt(index);

            if (CharacterManager.instance.EnemyButtons.Contains(btn))
            {
                CharacterManager.instance.EnemyButtons.Remove(btn);
                
                enemyButtons.Remove(btn);
                for (int j = 0; j < enemyButtons.Count; j++)
                {
                    int localIndex = j;
                    enemyButtons[j].onClick.RemoveAllListeners();
                    enemyButtons[j].onClick.AddListener(() => OnEnemySelected(localIndex));
                }
            }
           
        }

        if (!isEnemy)
        {
            foreach (var character in CharacterManager.instance.PlayerCharacters)
            {
                if (character.index >= index)
                {
                    character.index--;
                }
            }
        }
        else
        {
            foreach (var character in CharacterManager.instance.EnemyCharacters)
            {
                if (character.index >= index)
                {
                    character.index--;
                }
            }
            
        }
    }


    private void CheckBattleOutcome()
    {
        // 如果玩家全部死亡，则战斗失败
        if (CharacterManager.instance.PlayerCharacters.Count == 0)
        {
            Debug.Log("所有玩家角色都已死亡，战斗失败！");
            EndBattle(); // 结束战斗或回到主界面
            return;
        }
    
        // 如果敌人全部死亡，则战斗胜利
        if (CharacterManager.instance.EnemyCharacters.Count == 0)
        {
            Debug.Log("所有敌人角色都已死亡，战斗胜利！");
            EndBattle(); // 或者进入奖励界面等
            return;
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
