using System.Collections.Generic;
using UnityEngine;

public class Andromeda : Character
{
    [Header("Range Ref")]
    [SerializeField] CircleCollider2D circleCollider;

    // Cooldownds
    [Header("Cooldowns")]
    [SerializeField] protected float normalCD = 0.5f;
    [SerializeField] protected float skillCD = 0.5f;
    [SerializeField] protected float eveCD = 0.5f;

    [Header("Attacks")]
    [SerializeField] AttackSpawner normalAttack;
    [SerializeField] AttackSpawner eveAttack;

    protected override void Update()
    {
        base.Update();

        if (!isSelected && activePlayer.inCombat)
        {
            if (internalNormalCD >= normalCD)
            {
                UseNormalAttack();
            }
        }
    }

    public override void FindEnemyTarget() 
    {
        if (enemiesInRange.Count == 0) return;

        float distance = circleCollider.radius;
        if (enemiesInRange[0] == null) return;
        GameObject min = enemiesInRange[0].gameObject;
        int minIndex = 0;

        int i = 0;

        foreach (Collider2D enemy in enemiesInRange)
        {
            if (enemy == null || min == null) return;
            if (
                Vector3.Distance(transform.position, enemy.transform.position)
                <
                Vector3.Distance(transform.position, min.transform.position)
                ) {
                min = enemy.gameObject;
                minIndex = i;
            }
            i++;
        }

        if (activePlayer.inCombat && enemiesInRange[minIndex] != null) 
            target = enemiesInRange[minIndex].gameObject;
        else
            target = null;
    }
    public override void UseNormalAttack() {
        if (internalNormalCD < normalCD || state == ENTITY_STATE.dead) return;

        base.UseNormalAttack();

        Vector3 dir = Vector3.zero;
        Vector3 trackPos;
        if (target != null)
        {
            trackPos = target.transform.position;
        } else
        {
            trackPos = this.transform.position + joystick.GetDirAsVec3() * -1.5f;
        }

        dir = trackPos - this.transform.position;
        dir.Normalize();

        normalAttack.SpawnAttack(dir);

        if (activePlayer.inCombat)
        {
            int otherHealVal = Mathf.FloorToInt(0.5f * attack);
            activePlayer.HealAllCharacters(otherHealVal);
        }
        
        internalNormalCD = 0.0f;
    }
    public override void UseEve()
    {
        if (internalEveCD < eveCD || state == ENTITY_STATE.dead || activePlayer.entropy < 1.0f) return;
        base.UseEve();

        Vector3 dir = Vector3.zero;
        Vector3 trackPos;
        if (target != null)
        {
            trackPos = target.transform.position;
        }
        else
        {
            trackPos = this.transform.position + joystick.GetDirAsVec3() * -1.5f;
        }

        dir = trackPos - this.transform.position;
        dir.Normalize();

        int damageBoost = Mathf.CeilToInt((0.5f + activePlayer.entropy + (eveLevel * 0.05f)) * attack);
        eveAttack.SpawnAttack(dir, damageBoost);

        activePlayer.ReviveAllCharacters();

        int otherHealVal = Mathf.CeilToInt((8.0f + activePlayer.entropy + (eveLevel * 0.05f)) * magic);
        activePlayer.HealAllCharacters(otherHealVal);

        // Entropy Drain
        activePlayer.DrainEntropy();

        internalEveCD = 0.0f;
    }
    public override void UseSkill() 
    {
        if (internalSkillCD < skillCD || state == ENTITY_STATE.dead) return;

        base.UseSkill();

        int healValue = Mathf.CeilToInt(magic * (1.3f + (skillLevel * 0.05f)));
        activePlayer.HealAllCharacters(healValue);

        int resBuffVal = Mathf.FloorToInt(magic * (0.3f + (skillLevel * 0.05f)));
        activePlayer.BuffAllCharacters(GameEntity.STATS.res, resBuffVal, 10.0f);

        // Entropy Gain
        activePlayer.AddEntropy();

        internalSkillCD = 0.0f;
    }
}
