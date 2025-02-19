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
    
    public virtual void ProcessBuffs(ICharacter character)
    {
        
    }



    public virtual void UseSkill(MonsterAttributes attacker, ICharacter defender, MonsterSkillAttributes skill)
    {
        float totalPhysicalDamage = 0f;
        float totalSoulDamage = 0f;
    
        Debug.Log($"⚔ {attacker.characterName} 使用技能 {skill.skillName} (消耗: {skill.skillCost}) 对 {defender.characterName} 造成伤害！");

        for (int i = 0; i < skill.physicalAttackCount; i++)
        {
            float levelDifference = Mathf.Abs(attacker.level - defender.level);
            float physicalDamageReduction = defender.physicalDefense / (defender.physicalDefense + levelDifference * attacker.damageX1 + attacker.damageX2);
            float physicalDamage = attacker.physicalAttack * skill.physicalDamage * (1 - physicalDamageReduction);
            totalPhysicalDamage += physicalDamage;
        }

        for (int i = 0; i < skill.soulAttackCount; i++)
        {
            float levelDifference = Mathf.Abs(attacker.level - defender.level);
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

        BattleManager.instance.ApplyDamage(defender, totalDamage, isCritical);

        ApplySkillEffects(attacker, defender, skill);
    }

 

    public virtual void ApplySkillEffects(MonsterAttributes attacker, ICharacter defender, MonsterSkillAttributes skill)
    {
        // **眩晕**
        if (skill.stunChance > 0 && Random.value < skill.stunChance)
        {
            defender.activeBuffs.Add(new BuffEffect("眩晕", BuffType.Stun, 1, 0, true));
            BattleManager.instance.ShowText(defender.index, "眩晕!", Color.yellow);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 眩晕！");
        }

        // **沉默**
        if (skill.silenceChance > 0 && Random.value < skill.silenceChance)
        {
            defender.activeBuffs.Add(new BuffEffect("沉默", BuffType.Silence, 1, 0, true));
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

        // **流血**
        if (skill.bleedChance > 0 && Random.value < skill.bleedChance)
        {
            defender.activeBuffs.Add(new BuffEffect("流血", BuffType.Bleed, 3, skill.bleedStacks, true));
            BattleManager.instance.ShowText(defender.index, $"流血 +{skill.bleedStacks}", Color.red);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 施加流血 {skill.bleedStacks} 层！");
        }

        // **灼烧**
        if (skill.burnChance > 0 && Random.value < skill.burnChance)
        {
            defender.activeBuffs.Add(new BuffEffect("灼烧", BuffType.Burn, 3, skill.burnStacks, true));
            BattleManager.instance.ShowText(defender.index, $"灼烧 +{skill.burnStacks}", Color.red);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 施加灼烧 {skill.burnStacks} 层！");
        }

        // **中毒**
        if (skill.poisonStacks != 0)
        {
            defender.activeBuffs.Add(new BuffEffect("中毒", BuffType.Poison, 3, skill.poisonStacks, true));
            BattleManager.instance.ShowText(defender.index, "中毒!", Color.green);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 中毒！");
        }
        
        // **醉酒**
        if (skill.drunkStacks != 0)
        {
            defender.activeBuffs.Add(new BuffEffect("醉酒", BuffType.Drunk, 3, skill.drunkStacks, true));
            BattleManager.instance.ShowText(defender.index, "醉酒!", Color.cyan);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 醉酒！");
        }
        
        // **嘲讽**
        if (skill.taunt)
        {
            defender.activeBuffs.Add(new BuffEffect("嘲讽", BuffType.Taunt, 2, 0, true));
            BattleManager.instance.ShowText(defender.index, "被嘲讽!", Color.magenta);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 嘲讽！");
        }
        
        // **反伤**
        if (skill.reflectDamage)
        {
            attacker.activeBuffs.Add(new BuffEffect("反伤", BuffType.Reflect, 2, 0.3f, false));
            BattleManager.instance.ShowText(attacker.index, "反伤!", Color.blue);
            Debug.Log($"{attacker.characterName} 进入反伤状态！");
        }
        
        // **跳过回合**
        if (skill.skipTurn)
        {
            defender.activeBuffs.Add(new BuffEffect("跳过回合", BuffType.SkipTurn, 1, 0, true));
            BattleManager.instance.ShowText(defender.index, "跳过回合!", Color.gray);
            Debug.Log($"{defender.characterName} 被 {attacker.characterName} 施加跳过回合！");
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
