using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Details")]
    [HideInInspector] public GameEntity thisAttacker;
    [SerializeField] protected float lifespan = 1.0f;
    [SerializeField] protected bool targetEnemy = true;
    [SerializeField] protected float speed = 0.5f;
    [SerializeField] protected bool ranged = true;
    [SerializeField] protected bool pierce = false;
    [SerializeField] protected bool trackTarget = false;
    [SerializeField] protected DamageCalculator.DAMAGE_TYPE dmgType = DamageCalculator.DAMAGE_TYPE.normal;

    [Header("Additional Attack")]
    [SerializeField] public bool hasAdditionalAttack = false;
    [SerializeField] public bool staticAttack = true;
    [SerializeField] public AttackSpawner nextAttackSpawner;
    [SerializeField] public Vector3 nextAttackDirection;

    private bool isMoving = false;
    private Vector3 movementDirection = new Vector3 (0, 0, 0);

    private float life = 0.0f;

    // Visuals
    private Animator animator;

    void Start()
    {
        //animator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) return;

        life += Time.deltaTime;

        if (life < lifespan)
        {
            if (hasAdditionalAttack)
            {
                this.transform.parent.transform.position += movementDirection * speed * Time.deltaTime;
            } else
            if (ranged) this.transform.position += movementDirection * speed * Time.deltaTime;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public virtual void TriggerAttack(Vector3 direction)
    {
        movementDirection = direction;

        float zAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, zAngle);
        isMoving = true;

        life = 0.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string targetTag = "Enemy";
        if (!targetEnemy)
        {
            targetTag = "Character";
        }

        if (collision.gameObject.CompareTag(targetTag) && !collision.isTrigger)
        {
            GameEntity target;
            int dmgVal;

            // Deal Damage
            target = collision.gameObject.GetComponent<GameEntity>();

            if (target == null) return;

            if (target.state != GameEntity.ENTITY_STATE.dead)
            {
                dmgVal = CalculateDamage(thisAttacker, target);
                target.AlterHP(dmgVal);

                // Additional attack effect stuff
                if (dmgType == DamageCalculator.DAMAGE_TYPE.holy)
                {
                    target.AddBuff(GameEntity.STATS.res, 0.2f, -1, 4.0f);
                }

                if (dmgType == DamageCalculator.DAMAGE_TYPE.cursed)
                {
                    if (dmgVal > 1) thisAttacker.AlterHP(dmgVal / 2);
                }

                // Spawn additional attack
                if (hasAdditionalAttack)
                {
                    nextAttackSpawner.SpawnAttack(nextAttackDirection);
                }

                // Spawn the damage num
                target.SpawnDamageNumber(dmgType, dmgVal);
            }

            // Remove dis object if it doesnt pierce
            if (!pierce)
            {
                Destroy(this.gameObject);
            }
        }
    }

    protected virtual int CalculateDamage(GameEntity attacker, GameEntity target)
    {
        if (target == null || attacker == null) return 0;
        return DamageCalculator.CalculateDamage(attacker, target, dmgType);
    }

}
