using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] protected float lifespan = 1.0f;
    [SerializeField] protected bool targetEnemy = true;
    [SerializeField] protected float speed = 0.5f;
    [SerializeField] protected bool ranged = true;
    [SerializeField] protected bool pierce = false;
    [SerializeField] protected DamageCalculator.DAMAGE_TYPE dmgType = DamageCalculator.DAMAGE_TYPE.normal;

    [Header("Additional Attack")]
    [SerializeField] public bool hasAdditionalAttack = false;
    [SerializeField] public bool staticAttack = true;
    [SerializeField] private AttackSpawner nextAttackSpawner;
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

        if (collision.gameObject.tag == targetTag && !collision.isTrigger)
        {
            if (targetEnemy)
            {
                Enemy target = collision.gameObject.GetComponent<Enemy>();
                target.AlterHP(CalculateDamage());
            }
            else
            {
                Character target = collision.gameObject.GetComponent<Character>();
                target.AlterHP(CalculateDamage());
            }

            if (hasAdditionalAttack)
            {
                nextAttackSpawner.SpawnAttack(nextAttackDirection);
            }

            if (!pierce)
            {
                Destroy(this.gameObject);
            }
        }
    }

    protected virtual int CalculateDamage()
    {
        return -10;
    }
}
