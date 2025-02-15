using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public string enemyName;
    public CharacterAttributes enemyAttributes;
    
    public virtual void TakeTurn()
    {
        if (enemyAttributes.energy >= enemyAttributes.skillAttributes.cost)
        {
            UseSkill();
            enemyAttributes.energy -= enemyAttributes.skillAttributes.cost;
        }
        else
        {
            Attack();
            if (enemyAttributes.energy < enemyAttributes.maxEnergy)
            {
                enemyAttributes.energy += 1;
            }
        }
        EndTurn();
    }

    protected virtual void UseSkill()
    {
        CharacterAttributes target = GetRandomPlayer();
        if (target == null) return;

        Debug.Log($"{enemyName} 使用技能 {enemyAttributes.skillAttributes.cost} 对 {target.characterName} 造成伤害！");
        BattleManager.instance.UseSkill(enemyAttributes, target, enemyAttributes.skillAttributes);
    }

    protected virtual void Attack()
    {
        CharacterAttributes target = GetRandomPlayer();
        if (target == null) return;

        Debug.Log($"{enemyName} 攻击 {target.characterName}！");
        BattleManager.instance.DealDamage(enemyAttributes, target, 1f);
    }

    protected virtual void EndTurn()
    {
        BattleManager.instance.EndTurn();
    }

    protected CharacterAttributes GetRandomPlayer()
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