using System.Collections;
using UnityEngine;

public class Shai : Character
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
    [SerializeField] AttackSpawner skillAttack;
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
                )
            {
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
        }
        else
        {
            trackPos = this.transform.position + joystick.GetDirAsVec3() * -1.5f;
        }

        dir = trackPos - this.transform.position;
        dir.Normalize();

        normalAttack.SpawnAttack(dir);

        internalNormalCD = 0.0f;
    }
    public override void UseEve() 
    {
        if (internalEveCD < eveCD || state == ENTITY_STATE.dead) return;
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

        StartCoroutine(InternalEve(dir));

        // Atk buff
        int atkBuffVal = Mathf.FloorToInt(magic * (0.3f));
        activePlayer.BuffAllCharacters(GameEntity.STATS.atk, atkBuffVal, 10.0f);
        activePlayer.BuffAllCharacters(GameEntity.STATS.res, atkBuffVal, 10.0f);

        internalEveCD = 0.0f;
    }
    public override void UseSkill() 
    {
        if (internalSkillCD < skillCD || state == ENTITY_STATE.dead) return;
        base.UseSkill();

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

        skillAttack.SpawnAttack(dir);

        // Atk buff
        int atkBuffVal = Mathf.FloorToInt(magic * (0.3f));
        activePlayer.BuffAllCharacters(GameEntity.STATS.atk, atkBuffVal, 6.0f);

        internalSkillCD = 0.0f;
    }

    private IEnumerator InternalEve(Vector3 dir)
    {
        int damageBoost = Mathf.CeilToInt(0.5f * magic);
        for (int i = 0; i < 3; i++)
        {
            eveAttack.SpawnAttack(dir, damageBoost);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
