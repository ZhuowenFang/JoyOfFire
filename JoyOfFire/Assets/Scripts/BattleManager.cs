using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject buffIconPrefab;

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
    private bool skippingTurn = false;
    private SkillAttributes selectedSkill;
    public bool canUseSkill = true;
    private bool enemyHasTaunt = false;
    public ICharacter randomTarget;
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
        canUseSkill = true;
        actionPanel.gameObject.SetActive(false);
        ResetAllCharacterSizes();
        
        ICharacter nextCharacter = speedBarUI.GetNextCharacter();
        if (nextCharacter == null)
        {
            Debug.Log("没有角色进行回合！");
            return;
        }
        
        int characterIndex = nextCharacter.index;
        await Task.Delay(2000);
        if (characterIndex >= CharacterManager.instance.PlayerCharacters.Count)
        {
            actionPanel.gameObject.SetActive(false);
            characterButtons[characterIndex].transform.localScale = Vector3.one * 1.2f;
            await Task.Delay(2000);
            ProcessBuffs(nextCharacter);

            BaseEnemy enemyScript = characterButtons[characterIndex].GetComponent<BaseEnemy>();
            if (enemyScript != null)
            {
                if (skippingTurn)
                {
                    await Task.Delay(1000);
                    skippingTurn = false;
                    EndTurn();
                    return;
                }
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
            ProcessBuffs(nextCharacter);

            if (skippingTurn)
            {
                await Task.Delay(1000);
                skippingTurn = false;
                EndTurn();
                return;
            }
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
            if (!canUseSkill)
            {
                skillButton.interactable = false;
            }
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
        enemyHasTaunt = enemyButtons.Any(enemy =>
        {
            var enemyData = enemy.GetComponent<BaseEnemy>()?.enemyAttributes;
            return enemyData != null && enemyData.activeBuffs.Any(buff => buff.buffType == BuffType.Taunt);
        });

        foreach (var enemy in enemyButtons)
        {
            var enemyData = enemy.GetComponent<BaseEnemy>()?.enemyAttributes;
            if (enemyData != null && enemyData.activeBuffs.Any(buff => buff.buffType == BuffType.Stealth))
            {
                enemy.interactable = false;
                enemy.transform.localScale = Vector3.one;
            }
            else if (enemyHasTaunt)
            {
                if (enemyData == null || !enemyData.activeBuffs.Any(buff => buff.buffType == BuffType.Taunt))
                {
                    enemy.interactable = false;
                    enemy.transform.localScale = Vector3.one;
                }
                else
                {
                    enemy.interactable = true;
                }
            }
            else
            {
                enemy.interactable = true;
            }
        }

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
            var enemyData = enemy.GetComponent<BaseEnemy>()?.enemyAttributes;
            if (enemyData != null && enemyData.activeBuffs.Any(buff => buff.buffType == BuffType.Stealth))
            {
                enemy.transform.localScale = Vector3.one;
            }
            else if (enemyHasTaunt)
            {
                if (enemyData != null && enemyData.activeBuffs.Any(buff => buff.buffType == BuffType.Taunt))
                {
                    enemy.transform.localScale = Vector3.one * scale;
                }
                else
                {
                    enemy.transform.localScale = Vector3.one;
                }
            }
            else
            {
                enemy.transform.localScale = Vector3.one * scale;
            }
        }
    }


    public void OnEnemySelected(int enemyIndex)
    {
        if (!isSelectingEnemy) return;
        Debug.Log($"选择了敌人 {enemyIndex}！");

        ICharacter attacker = SpeedBarUI.instance.GetNextCharacter();
        ICharacter defender = CharacterManager.instance.EnemyCharacters[enemyIndex];

        if (randomTarget != null)
        {
            defender = randomTarget;
            randomTarget = null;
            bool targetIsPlayer;
            if (defender is MonsterAttributes)
            {
                targetIsPlayer = false;
            }
            else
            {
                targetIsPlayer = true;
            }
            if (isUsingSkill)
            {
                UseSkill(attacker, defender, selectedSkill,targetIsPlayer);
                attacker.energy -= selectedSkill.skillCost;
            }
            else
            {
                DealDamage(attacker, defender, 1f,targetIsPlayer);
                if (attacker.energy < attacker.maxEnergy)
                {
                    attacker.energy += 1;
                }
            }

            isSelectingEnemy = false;
            ResetAllCharacterSizes();
            EndTurn();
        }
        else
        {
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
        
    }
    
    public void DealDamage(ICharacter attacker, ICharacter defender, float damageMultiplier, bool targetIsPlayer = false)
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

        ApplyDamage(attacker,defender, damage, isCritical, targetIsPlayer);
    }

    public void ApplyDamage(ICharacter IfReflectCharacter, ICharacter defender, float damage, bool isCritical, bool targetIsPlayer = false)
    {
        float remainingShield = Mathf.Max(defender.shieldAmount - damage, 0);
        float damageToHealth = Mathf.Max(damage - defender.shieldAmount, 0);
        defender.shieldAmount = remainingShield;
        defender.currentHealth -= damageToHealth;
        Debug.Log($"{defender.characterName} 受到了 {damageToHealth} 点伤害！");
        Debug.Log($"敌人现在的生命值：{defender.currentHealth}");
        ShowDamageText(defender.index, damageToHealth, isCritical, targetIsPlayer);
        UpdateButtonHealthFill();

        if (defender.activeBuffs.Exists(b => b.buffType == BuffType.Reflect))
        {
            float reflectedDamage = damageToHealth * 0.5f;
            float attackerRemainingShield = Mathf.Max(IfReflectCharacter.shieldAmount - reflectedDamage, 0);
            float damageToAttackerHealth = Mathf.Max(reflectedDamage - IfReflectCharacter.shieldAmount, 0);
            IfReflectCharacter.shieldAmount = attackerRemainingShield;
            IfReflectCharacter.currentHealth -= damageToAttackerHealth;
            Debug.Log($"{IfReflectCharacter.characterName} 受到了 {damageToAttackerHealth} 点反弹伤害！");
            ShowDamageText(IfReflectCharacter.index, damageToAttackerHealth, false, true);
            UpdateButtonHealthFill(); // 根据需要更新攻击者的UI
        }

        if (defender.currentHealth <= 0)
        {
            Debug.Log($"{defender.characterName} 被击败！");
            RemoveCharacterFromBattle(defender);
            CheckBattleOutcome();
        }
    }

    
    public void UseSkill(ICharacter attacker, ICharacter defender, SkillAttributes skill, bool targetIsPlayer = false)
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
        ApplyDamage(attacker, defender,totalDamage,isCritical,targetIsPlayer);

        // 技能附加效果
        ApplySkillEffects(attacker, defender, skill);
    }
    
    public void AddBuff(ICharacter attcker, ICharacter defender, string name, BuffType buffType, int turns, float stackVal, bool debuff, int maxStacksVal)
    {
        BuffEffect existingBuff = defender.activeBuffs.Find(b => b.buffType == buffType);
        if (existingBuff != null)
        {   
            if (existingBuff.stack + stackVal >= maxStacksVal)
            {
                existingBuff.stack = maxStacksVal;
            }
            else
            {
                existingBuff.stack += stackVal;
            }
            Debug.Log($"增加了 {name} 的层数，当前层数：{existingBuff.stack}");
        }
        else
        {
            BuffEffect newBuff = new BuffEffect(attcker, name, buffType, turns, stackVal, debuff, maxStacksVal);
            defender.activeBuffs.Add(newBuff);
        }
        Button targetButton = null;
        targetButton = CharacterManager.instance.characterButtons[defender.index];
        
        if (targetButton == null)
        {
            Debug.LogError("找不到对应的角色按钮");
            return;
        }
        
        Transform buffGroup = targetButton.transform.Find("BuffGroup");
        if (buffGroup == null)
        {
            Debug.LogError("未在按钮下找到 BuffGroup 子物体");
            return;
        }
        
        BuffIcon[] existingIcons = buffGroup.GetComponentsInChildren<BuffIcon>();
        BuffIcon matchingIcon = null;
        foreach (var icon in existingIcons)
        {
            if (icon.buffType == buffType)
            {
                matchingIcon = icon;
                break;
            }
        }
        
        if (matchingIcon != null)
        {
            Text stackText = matchingIcon.transform.Find("Stack").GetComponent<Text>();
            if (stackText != null)
            {
                if (int.Parse(stackText.text) + stackVal >= maxStacksVal)
                {
                    stackText.text = maxStacksVal.ToString();
                }
                else
                {
                    stackText.text = (int.Parse(stackText.text) + stackVal).ToString();
                }
                Debug.Log($"增加了图标 {name} 的层数，当前层数：{stackText.text}");

            }
            else
            {
                Debug.LogError("未找到 BuffIcon 下的 Stack 文本组件");
            }
        }
        else
        {
            GameObject buffIconGO = Instantiate(buffIconPrefab, buffGroup);
            BuffIcon buffIconScript = buffIconGO.GetComponent<BuffIcon>();
            if (buffIconScript != null)
            {
                buffIconScript.buffType = buffType;
            }
            
            Sprite buffSprite = Resources.Load<Sprite>("buffIcons/" + buffType.ToString());
            if (buffSprite != null)
            {
                Image iconImage = buffIconGO.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = buffSprite;
                }
                else
                {
                    Debug.LogError("buffIconPrefab 上没有找到 Image 组件");
                }
            }
            else
            {
                Debug.LogError("无法加载 buffSprite，检查路径或文件名: " + buffType.ToString());
            }
            
            Transform stackTransform = buffIconGO.transform.Find("Stack");
            if (stackTransform != null)
            {
                Text stackText = stackTransform.GetComponent<Text>();
                if (stackText != null)
                {
                    stackText.text = stackVal.ToString();
                }
                else
                {
                    Debug.LogError("未找到 Stack 子物体上的 Text 组件");
                }
            }
            else
            {
                Debug.LogError("未找到 buffIconPrefab 下的 Stack 子物体");
            }
        }
    }
    
    public void ProcessBuffs(ICharacter character)
    {
        List<BuffEffect> buffsToRemove = new List<BuffEffect>();

        foreach (var buff in character.activeBuffs)
        {
            switch (buff.buffType)
            {
                case BuffType.Stun:
                    skippingTurn = true;
                    buffsToRemove.Add(buff);
                    break;
                case BuffType.Silence:
                    canUseSkill = false;
                    buffsToRemove.Add(buff);
                    break;
                case BuffType.Block:
                    buffsToRemove.Add(buff);
                    break;
                case BuffType.Bleed:
                    float bleedDamage = buff.stack * 0.01f * character.health;
                    ApplyDamage(character,character, bleedDamage, false, true);
                    buff.stack = Mathf.FloorToInt(buff.stack / 3);
                    if (buff.stack <= 0)
                    {
                        buffsToRemove.Add(buff);
                    }
                    else
                    {
                        UpdateBuffIconStack(character, buff.buffType, buff.stack);
                    }
                    break;
                case BuffType.Burn:
                    float burnDamage = buff.stack * 0.01f * character.physicalAttack;
                    ApplyDamage(character,character, burnDamage, false,true);
                    buff.stack = Mathf.FloorToInt(buff.stack / 3);
                    if (buff.stack <= 0)
                    {
                        buffsToRemove.Add(buff);
                    }
                    else
                    {
                        UpdateBuffIconStack(character, buff.buffType, buff.stack);
                    }
                    break;
                case BuffType.SkipTurn:
                    skippingTurn = true;
                    buffsToRemove.Add(buff);
                    break;
                case BuffType.Gaze:
                    float gazeDamage = buff.stack;
                    ApplyDamage(character,character, gazeDamage, false,true);
                    buff.stack = Mathf.FloorToInt(buff.stack / 2);
                    if (buff.stack <= 0)
                    {
                        buffsToRemove.Add(buff);
                    }
                    else
                    {
                        UpdateBuffIconStack(character, buff.buffType, buff.stack);
                    }
                    break;
                case BuffType.Poison:
                    float poisonDamage = buff.stack;
                    ApplyDamage(character,character, poisonDamage, false,true);
                    buff.stack = Mathf.FloorToInt(buff.stack / 2);
                    if (buff.stack <= 0)
                    {
                        buffsToRemove.Add(buff);
                    }
                    else
                    {
                        UpdateBuffIconStack(character, buff.buffType, buff.stack);
                    }
                    break;
                case BuffType.Taunt:
                    buffsToRemove.Add(buff);
                    break;
                case BuffType.Drunk:
                    List<ICharacter> potentialTargets = NewCharacterManager.instance.allCharacters
                        .Where(c => c != character)
                        .ToList();
                    if (potentialTargets.Count > 0 && Random.value < buff.stack * 0.2f)
                    {
                        int randomIndex = Random.Range(0, potentialTargets.Count);
                        randomTarget = potentialTargets[randomIndex];
                    }
                    buff.stack--;
                    if (buff.stack <= 0)
                    {
                        buffsToRemove.Add(buff);
                    }
                    else
                    {
                        UpdateBuffIconStack(character, buff.buffType, buff.stack);
                    }
                    break;
                case BuffType.Reflect:
                    buffsToRemove.Add(buff);
                    break;
                case BuffType.Stealth:
                    buffsToRemove.Add(buff);
                    break;
                
            }
        }

        foreach (var buff in buffsToRemove)
        {
            character.activeBuffs.Remove(buff);
            RemoveBuffIcon(character, buff.buffType);
        }
    }


    public void RemoveBuffIcon(ICharacter character, BuffType buffType)
    {
        Button targetButton = null;
        targetButton = CharacterManager.instance.characterButtons[character.index];

        if (targetButton == null)
        {
            Debug.LogError("未找到对应的角色按钮");
            return;
        }

        Transform buffGroup = targetButton.transform.Find("BuffGroup");
        if (buffGroup == null)
        {
            Debug.LogError("未在角色按钮下找到 BuffGroup");
            return;
        }

        BuffIcon[] icons = buffGroup.GetComponentsInChildren<BuffIcon>();
        foreach (var icon in icons)
        {
            if (icon.buffType == buffType)
            {
                Destroy(icon.gameObject);
                break;
            }
        }
    }
    public void UpdateBuffIconStack(ICharacter character, BuffType buffType, float newStack)
    {
        Button targetButton = CharacterManager.instance.characterButtons[character.index];
        if (targetButton == null)
        {
            Debug.LogError("找不到对应的角色按钮");
            return;
        }

        Transform buffGroup = targetButton.transform.Find("BuffGroup");
        if (buffGroup == null)
        {
            Debug.LogError("未在角色按钮下找到 BuffGroup 子物体");
            return;
        }

        BuffIcon[] icons = buffGroup.GetComponentsInChildren<BuffIcon>();
        foreach (var icon in icons)
        {
            if (icon.buffType == buffType)
            {
                Transform stackTransform = icon.transform.Find("Stack");
                if (stackTransform != null)
                {
                    Text stackText = stackTransform.GetComponent<Text>();
                    if (stackText != null)
                    {
                        stackText.text = newStack.ToString();
                        Debug.Log($"更新图标 {buffType} 的层数为：{stackText.text}");
                    }
                    else
                    {
                        Debug.LogError("未找到 BuffIcon 下的 Stack 文本组件");
                    }
                }
                else
                {
                    Debug.LogError("未找到 BuffIcon 下的 Stack 子物体");
                }
                break;
            }
        }
    }


    private void ApplySkillEffects(ICharacter attacker, ICharacter defender, SkillAttributes skill)
    {
        // **眩晕**
        if (skill.stunChance > 0 && Random.value < skill.stunChance)
        {
            AddBuff(attacker, defender, "眩晕", BuffType.Stun, 1, 1, true,1);
        }

        // **沉默**
        if (skill.silenceChance > 0 && Random.value < skill.silenceChance)
        {
            AddBuff(attacker, defender, "沉默", BuffType.Silence, 1, 1, true,1);

        }

        // **增加护盾**
        if (skill.shieldAmount > 0)
        {
            attacker.shieldAmount += skill.shieldAmount;
        }

        // **治疗**
        if (skill.healAmount > 0)
        {
            float healDiscount = 1f;
            if(InventoryManager.instance.activeItems.ContainsKey("Gold_plate"))
            {
                healDiscount = 0.8f;
            }
            attacker.currentHealth = Mathf.Min(attacker.currentHealth + skill.healAmount * healDiscount, attacker.health);
        }

        // **格挡**
        if (skill.blockChance > 0 && Random.value < skill.blockChance)
        {
            AddBuff(attacker, defender, "格挡", BuffType.Block, 1, 1, false,1);
        }
        UpdateButtonHealthFill();
    }



    private void ShowDamageText(int characterIndex, float damage, bool isCritical, bool targetIsPlayer = false)
    {
        RectTransform targetRect = CharacterManager.instance.characterButtons[characterIndex].GetComponent<RectTransform>();

        GameObject damageText = Instantiate(damageTextPrefab, mainCanvas.transform);
        if (isCritical)
        {
            damageText.GetComponent<Text>().text = $"暴击！-{damage:F0}";
        }
        else
        {
            damageText.GetComponent<Text>().text = $"-{damage:F0}";

        }

        RectTransform textRect = damageText.GetComponent<RectTransform>();
        textRect.SetParent(mainCanvas.transform, false);
        if (!targetIsPlayer)
        {
            textRect.localPosition = targetRect.localPosition + new Vector3(0, 250, 0);
        }
        else
        {
            textRect.localPosition = targetRect.localPosition + new Vector3(0, 130, 0);

        }
        

        damageText.transform.SetAsLastSibling();

        
        Destroy(damageText, 1f);
    }
    

    public void ShowText(int characterIndex, string text,Color color, bool targetIsPlayer = false)
    {
        RectTransform targetRect = CharacterManager.instance.characterButtons[characterIndex].GetComponent<RectTransform>();

        GameObject damageText = Instantiate(damageTextPrefab, mainCanvas.transform);
        
        damageText.GetComponent<Text>().text = text;
        damageText.GetComponent<Text>().color = color;
        
        RectTransform textRect = damageText.GetComponent<RectTransform>();
        textRect.SetParent(mainCanvas.transform, false);
        if (!targetIsPlayer)
        {
            textRect.localPosition = targetRect.localPosition + new Vector3(0, 250, 0);
        }
        else
        {
            textRect.localPosition = targetRect.localPosition + new Vector3(0, 130, 0);

        }
        

        damageText.transform.SetAsLastSibling();

        
        Destroy(damageText, 1f);
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
            button.interactable = true;
        }
    }
    private void RemoveCharacterFromBattle(ICharacter deadCharacter)
    {
        // 根据 deadCharacter 类型判断是玩家还是敌人
        if (deadCharacter is CharacterAttributes)
        {
            CharacterAttributes deadPlayer = deadCharacter as CharacterAttributes;
            CharacterManager.instance.PlayerCharacters.Remove(deadPlayer);
            RemoveCharacterButton(deadPlayer.index, false);
        }
        else if (deadCharacter is MonsterAttributes)
        {
            MonsterAttributes deadMonster = deadCharacter as MonsterAttributes;
            CharacterManager.instance.EnemyCharacters.Remove(deadMonster);
            RemoveCharacterButton(deadMonster.index, true);
        }
    
        speedBarUI.RemoveCharacter(deadCharacter);
    }
    public void UpdateButtonHealthFill()
    {
        for (int i = 0; i < CharacterManager.instance.PlayerCharacters.Count; i++)
        {
            CharacterAttributes player = CharacterManager.instance.PlayerCharacters[i];
            Button btn = CharacterManager.instance.characterButtons[i];
            Image fillImage = btn.transform.Find("fill").GetComponent<Image>();
            if (player.health > 0)
            {
                fillImage.fillAmount = player.currentHealth / player.health;
            }
        }

        for (int i = 0; i < CharacterManager.instance.EnemyCharacters.Count; i++)
        {
            MonsterAttributes enemy = CharacterManager.instance.EnemyCharacters[i];
            Button btn = CharacterManager.instance.EnemyButtons[i];
            Image fillImage = btn.transform.Find("fill").GetComponent<Image>();
            if (enemy.health > 0)
            {
                fillImage.fillAmount = enemy.currentHealth / enemy.health;
            }
        }
    }

    /// <summary>
    /// 根据索引移除角色按钮（销毁对应UI对象，并从列表中移除）。
    /// </summary>
    private void RemoveCharacterButton(int index, bool ButtonIsEnemy)
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

        if (!ButtonIsEnemy)
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

        List<ICharacter> toRemove = new List<ICharacter>();

        foreach (ICharacter character in NewCharacterManager.instance.allCharacters)
        {
            if (character is CharacterAttributes player)
            {
                player.currentHealth = player.health;
                player.energy = 0;
                player.activeBuffs?.Clear();
            }
            else
            {
                toRemove.Add(character);
            }
        }

        // 退出 foreach 后再移除
        foreach (ICharacter dead in toRemove)
        {
            NewCharacterManager.instance.allCharacters.Remove(dead);
        }

        SceneManager.UnloadSceneAsync("Battle");
    }

}
