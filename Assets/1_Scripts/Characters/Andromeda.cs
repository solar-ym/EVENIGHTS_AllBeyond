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

    // Internal CD
    private float internalNormalCD = 0.0f;
    private float internalSkillCD = 0.0f;
    private float internalEveCD = 0.0f;

    protected override void Update()
    {
        base.Update();


    }

    protected override GameObject FindEnemyTarget() 
    {
        float distance = circleCollider.radius;
        GameObject min = enemiesInRange[0].gameObject;

        foreach (Collider2D enemy in enemiesInRange)
        {
            if (
                Vector3.Distance(transform.position, enemy.transform.position)
                <
                Vector3.Distance(transform.position, min.transform.position)
                ) {
                min = enemy.gameObject;
            }
        }

        return min;
    }
    public override void UseNormalAttack() { }
    public override void UseEve() { }
    public override void UseSkill() { }
}
