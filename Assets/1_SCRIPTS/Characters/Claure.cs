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
    [SerializeField] AttackSpawner specialNormalAttack;
    [SerializeField] AttackSpawner eveAttack;

    // Special state
    private bool isInSpecialEveState = false;
    private float specialCounter = 0.0f;
    private float specialDuration = 10.0f;

    protected override void Update()
    {
        base.Update();

        if (isInSpecialEveState)
        {
            specialCounter += Time.deltaTime;
            if (specialCounter >= specialDuration)
            {
                specialCounter = 0.0f;
                isInSpecialEveState = false;
            }
        }

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

        FindEnemyTarget();
        movementLock = true;

        ResetAnimTriggers();

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

        if (!isInSpecialEveState)
        {
            normalAttack.SpawnAttack(dir);
            animator.SetTrigger("Normal");
        } else
        {
            int damageBoost = Mathf.CeilToInt(1.5f + (eveLevel * 0.05f) * magic);
            specialNormalAttack.SpawnAttack(dir,  damageBoost);
            animator.SetTrigger("Eve");
        }

            internalNormalCD = 0.0f;
    }
    public override void UseEve()
    {
        if (internalEveCD < eveCD || state == ENTITY_STATE.dead || activePlayer.entropy == 0) return;
        base.UseEve();

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

        int damageBoost = Mathf.CeilToInt((2.0f + (eveLevel * 0.05f) + activePlayer.entropy) * magic);
        eveAttack.SpawnAttack(dir);

        isInSpecialEveState = true;

        // Entropy Drain
        activePlayer.DrainEntropy();

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
            trackPos = this.transform.position + joystick.GetDirAsVec3();
        }

        dir = trackPos - this.transform.position;
        dir.Normalize();

        skillAttack.SpawnAttack(dir);

        // Buff res
        int resBuffVal = Mathf.FloorToInt(magic * (0.6f + (skillLevel * 0.05f)));
        AddBuff(STATS.res, resBuffVal, 1, 3.0f);

        // Entropy Gain
        activePlayer.AddEntropy();

        internalSkillCD = 0.0f;
    }
}
