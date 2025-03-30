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

        foreach (var skill in enemyAttributes.skills)
        {
            if (enemyAttributes.energy >= skill.skillCost)
            {
                UseSkill(enemyAttributes, GetRandomPlayer(), skill);
                enemyAttributes.energy -= skill.skillCost;
                EndTurn();
                return;
            }
        }

        Attack();
        if (enemyAttributes.energy < enemyAttributes.maxEnergy)
        {
            enemyAttributes.energy += 1;
        }

        EndTurn();
    }


    public virtual void UseSkill(MonsterAttributes attacker, ICharacter defender, MonsterSkillAttributes skill)
    {
        float totalPhysicalDamage = 0f;
        float totalSoulDamage = 0f;
    
        Debug.Log($"⚔ {attacker.characterName} 使用技能 {skill.skillName} (消耗: {skill.skillCost}) 对 {defender.characterName} 造成伤害！");

        // 计算多段 **物理伤害**
        for (int i = 0; i < skill.physicalAttackCount; i++)
        {
            float levelDifference = Mathf.Abs(attacker.level - defender.level);
            float physicalDamageReduction = defender.physicalDefense / (defender.physicalDefense + levelDifference * attacker.damageX1 + attacker.damageX2);
            float physicalDamage = attacker.physicalAttack * skill.physicalDamage * (1 - physicalDamageReduction);
            totalPhysicalDamage += physicalDamage;
        }

        // 计算多段 **灵魂伤害**
        for (int i = 0; i < skill.soulAttackCount; i++)
        {
            float levelDifference = Mathf.Abs(attacker.level - defender.level);
            float soulDamageReduction = defender.soulDefense / (defender.soulDefense + levelDifference * attacker.damageX1 + attacker.damageX2);
            float soulDamage = attacker.soulAttack * skill.soulDamage * (1 - soulDamageReduction);
            totalSoulDamage += soulDamage;
        }

        // **总伤害 = 物理 + 灵魂**
        float totalDamage = totalPhysicalDamage + totalSoulDamage;

        // **暴击计算**
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

        // **应用伤害**
        BattleManager.instance.ApplyDamage(defender, totalDamage, isCritical);

        // **应用技能效果**
        ApplySkillEffects(attacker, defender, skill);
    }

 
    private float CalculateSkillDamage(MonsterAttributes attacker, MonsterSkillAttributes skill)
    {
        float physicalDamage = attacker.physicalAttack * skill.physicalDamage;
        float soulDamage = attacker.soulAttack * skill.soulDamage;
        return physicalDamage + soulDamage;
    }
    
    private void ApplySkillEffects(MonsterAttributes attacker, ICharacter defender, MonsterSkillAttributes skill)
    {
        // **眩晕**
        if (skill.stunChance > 0 && Random.value < skill.stunChance)
        {
            BattleManager.instance.ShowText(defender.index, "眩晕!", Color.yellow);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 眩晕！");
        }

        // **沉默**
        if (skill.silenceChance > 0 && Random.value < skill.silenceChance)
        {
            BattleManager.instance.ShowText(defender.index, "沉默!", Color.blue);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 沉默！");
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
    }



    protected virtual void Attack()
    {
        CharacterAttributes target = GetRandomPlayer();
        if (target == null) return;

        BattleManager.instance.DealDamage(enemyAttributes, target, 1f);
    }


    protected virtual void EndTurn()
    {
        BattleManager.instance.EndTurn();
    }
    
    protected virtual CharacterAttributes GetRandomPlayer()
    {
        List<CharacterAttributes> alivePlayers = new List<CharacterAttributes>();

        foreach (var player in CharacterManager.instance.PlayerCharacters)
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
