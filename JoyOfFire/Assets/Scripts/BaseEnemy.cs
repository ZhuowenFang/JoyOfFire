using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public string enemyName;
    [SerializeReference]
    public MonsterAttributes enemyAttributes;

    public virtual void TakeTurn()
    {
        Debug.Log($"【敌人行动】{enemyAttributes.characterName} (Index: {enemyAttributes.index}) 当前能量: {enemyAttributes.energy}");

        List<MonsterSkillAttributes> availableSkills = new List<MonsterSkillAttributes>();
        foreach (var skill in enemyAttributes.skills)
        {
            if (enemyAttributes.energy >= skill.skillCost && BattleManager.instance.canUseSkill)
            {
                availableSkills.Add(skill);
            }
        }

        if (availableSkills.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSkills.Count);
            MonsterSkillAttributes selectedSkill = availableSkills[randomIndex];

            UseSkill(enemyAttributes, GetRandomPlayer(), selectedSkill);
            enemyAttributes.energy -= selectedSkill.skillCost;
            EndTurn();
            return;
        }

        Attack();
        if (enemyAttributes.energy < enemyAttributes.maxEnergy)
        {
            enemyAttributes.energy += 1;
        }

        EndTurn();
    }

    
    public virtual void ProcessBuffs(ICharacter character)
    {
        
    }



    public virtual void UseSkill(MonsterAttributes attacker, ICharacter defender, MonsterSkillAttributes skill)
    {
        float totalPhysicalDamage = 0f;
        float totalSoulDamage = 0f;
    
        System.Reflection.FieldInfo[] fields = typeof(MonsterSkillAttributes).GetFields();
        foreach (var field in fields)
        {
            Debug.Log($"{field.Name} : {field.GetValue(skill)}");
        }
        for (int i = 0; i < skill.physicalAttackCount; i++)
        {
            float levelDifference = defender.level - attacker.level;
            float physicalDamageReduction = defender.physicalDefense / (defender.physicalDefense + levelDifference * attacker.damageX1 + attacker.damageX2);
            float physicalDamage = attacker.physicalAttack * skill.physicalDamage * (1 - physicalDamageReduction);
            totalPhysicalDamage += physicalDamage;
        }

        for (int i = 0; i < skill.soulAttackCount; i++)
        {
            float levelDifference = defender.level - attacker.level;
            float soulDamageReduction = defender.soulDefense / (defender.soulDefense + levelDifference * attacker.damageX1 + attacker.damageX2);
            float soulDamage = attacker.soulAttack * skill.soulDamage * (1 - soulDamageReduction);
            totalSoulDamage += soulDamage;
        }

        float totalDamage = totalPhysicalDamage + totalSoulDamage;

        float effectiveCriticalRate = attacker.criticalRate + skill.criticalChance;
        bool isCritical = Random.value < effectiveCriticalRate;
        if (isCritical)
        {
            totalDamage *= 1.5f;
            Debug.Log($"💥 暴击！{attacker.characterName} 对 {defender.characterName} 造成 {totalDamage:F0} 点伤害！");
        }
        else
        {
            Debug.Log($"⚔ {attacker.characterName} 对 {defender.characterName} 造成 {totalDamage:F0} 点伤害！");
        }

        BattleManager.instance.ApplyDamage(defender,defender, totalDamage, isCritical,true);

        ApplySkillEffects(attacker, defender, skill);
    }

 

    public virtual void ApplySkillEffects(MonsterAttributes attacker, ICharacter defender, MonsterSkillAttributes skill)
    {
        
        // **眩晕**
        if (skill.stunChance > 0 && Random.value < skill.stunChance)
        {
            BattleManager.instance.AddBuff(attacker, defender, "眩晕", BuffType.Stun, 1, 1, true,1);

        }

        // **沉默**
        if (skill.silenceChance > 0 && Random.value < skill.silenceChance)
        {
            BattleManager.instance.AddBuff(attacker, defender, "沉默", BuffType.Silence, 1, 1, true,1);
        }

        // **护盾**
        if (skill.shieldValue > 0)
        {
            attacker.shieldAmount += skill.shieldValue;
            BattleManager.instance.ShowText(attacker.index, $"护盾 +{skill.shieldValue}", Color.green);
            
            Debug.Log($"{attacker.characterName} 获得 {skill.shieldValue} 点护盾！");
        }

        // **治疗**
        if (skill.fixedHeal > 0 || skill.percentHeal > 0)
        {
            float healAmount = skill.fixedHeal + (attacker.health * skill.percentHeal);
            attacker.currentHealth = Mathf.Min(attacker.currentHealth + healAmount, attacker.health);
            BattleManager.instance.ShowText(attacker.index, $"治疗 +{healAmount:F0}", Color.green);
            Debug.Log($"{attacker.characterName} 回复 {healAmount} 点生命值！");
        }

        // **吸血**
        if (skill.fixedLifesteal > 0 || skill.percentLifesteal > 0)
        {
            float lifestealAmount = skill.fixedLifesteal + (defender.health * skill.percentLifesteal);
            attacker.currentHealth = Mathf.Min(attacker.currentHealth + lifestealAmount, attacker.health);
            BattleManager.instance.ShowText(attacker.index, $"吸血 +{lifestealAmount:F0}", Color.red);
            Debug.Log($"{attacker.characterName} 吸血 {lifestealAmount} 点！");
        }

        // **流血**
        if (skill.bleedChance > 0 && Random.value < skill.bleedChance)
        {
            BattleManager.instance.AddBuff(attacker, defender, "流血", BuffType.Bleed, 1, skill.bleedStacks, true,25);
            Debug.Log($"{attacker.characterName} 对 {defender.characterName} 造成流血效果！");
            
        }

        // **灼烧**
        if (skill.burnChance > 0 && Random.value < skill.burnChance)
        {
            BattleManager.instance.AddBuff(attacker, defender, "灼烧", BuffType.Burn, 1, skill.burnStacks, true,25);

        }

        // **中毒**
        if (skill.poisonStacks != 0)
        {
            BattleManager.instance.AddBuff(attacker, defender, "中毒", BuffType.Poison, 1, skill.poisonStacks, true, 25);

        }
        // **醉酒**
        if (skill.drunkStacks != 0)
        {
            BattleManager.instance.AddBuff(attacker, defender, "醉酒", BuffType.Drunk, 1, skill.drunkStacks, true, 5);
        }
        
        // **嘲讽**
        if (skill.taunt)
        {
            BattleManager.instance.AddBuff(attacker, attacker, "嘲讽", BuffType.Taunt, 1, 1, true,1);

        }
        
        // **反伤**
        if (skill.reflectDamage)
        {
            BattleManager.instance.AddBuff(attacker, attacker, "反伤", BuffType.Reflect, 1, 1, true,1);
  
        }
        
        // **跳过回合**
        if (skill.skipTurn)
        {
            BattleManager.instance.AddBuff(attacker, defender, "跳过回合", BuffType.SkipTurn, 1, 1, true,1);
        }
        
        // **凝视**
        if (skill.gazeStacks != 0)
        {
            BattleManager.instance.AddBuff(attacker, defender, "凝视", BuffType.Gaze, 1, skill.gazeStacks, true, 13);
        }
        
        // **宰杀**
        if (skill.executeTarget)
        {
            BattleManager.instance.AddBuff(attacker, defender, "宰杀", BuffType.Execute, 1, 1, true,1);
        }
        
        // **潜行**
        if (skill.stealth)
        {
            BattleManager.instance.AddBuff(attacker, attacker, "潜行", BuffType.Stealth, 1, 1, true,1);
        }
        
        
        BattleManager.instance.UpdateButtonHealthFill();
        BattleManager.instance.UpdateButtonSheildFill();

        
        
    }




    protected virtual void Attack()
    {
        CharacterAttributes target = GetRandomPlayer();
        if (target == null) return;

        BattleManager.instance.DealDamage(enemyAttributes, target, 1f, true);
    }


    protected virtual void EndTurn()
    {
        BattleManager.instance.EndTurn();
    }
    
    protected virtual CharacterAttributes GetRandomPlayer()
    {
        List<CharacterAttributes> alivePlayers = new List<CharacterAttributes>();

        foreach (var player in BattleCharacterManager.instance.PlayerCharacters)
        {
            if (player.currentHealth > 0)
            {
                alivePlayers.Add(player);
            }
        }

        if (alivePlayers.Count == 0)
        {
            Debug.LogWarning("没有存活的玩家可供攻击！");
            return null;
        }

        int randomIndex = Random.Range(0, alivePlayers.Count);
        return alivePlayers[randomIndex];
    }
}
