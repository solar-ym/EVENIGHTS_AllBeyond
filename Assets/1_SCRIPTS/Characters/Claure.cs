using UnityEngine;

public class Claure : Character
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


    protected override void Update()
    {
        base.Update();

        if (!isSelected && activePlayer.inCombat)
        {
            if (internalNormalCD >= normalCD && enemiesInRange.Count > 0)
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
        GameObject max = enemiesInRange[0].gameObject;
        int maxIndex = 0;

        int i = 0;

        foreach (Collider2D enemy in enemiesInRange)
        {
            if (enemy == null || max == null) return;
            if (
                Vector3.Distance(transform.position, enemy.transform.position)
                >
                Vector3.Distance(transform.position, max.transform.position)
                )
            {
                max = enemy.gameObject;
                maxIndex = i;
            }
            i++;
        }

        if (activePlayer.inCombat && enemiesInRange[maxIndex] != null)
            target = enemiesInRange[maxIndex].gameObject;
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
            trackPos = this.transform.position + joystick.GetDirAsVec3();
        }

        dir = trackPos - this.transform.position;
        dir.Normalize();

        normalAttack.SpawnAttack(dir);

        internalNormalCD = 0.0f;
    }
    public override void UseEve() { }
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
            trackPos = this.transform.position + joystick.GetDirAsVec3();
        }

        dir = trackPos - this.transform.position;
        dir.Normalize();

        skillAttack.SpawnAttack(dir);

        // Buff res
        int resBuffVal = Mathf.FloorToInt(magic * (0.6f));
        AddBuff(STATS.res, resBuffVal, 1, 3.0f);

        internalSkillCD = 0.0f;
    }
}
