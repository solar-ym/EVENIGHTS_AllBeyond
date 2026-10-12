using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using static Enemy;

public class Character : GameEntity
{
    [Header("Movement")]
    [SerializeField] protected FixedJoystick joystick;
    [SerializeField] private float movementSpeed = 2.5f;
    [SerializeField] private float trackDistance = 1.5f;
    [SerializeField] private float enemyTrackDistance = 1.5f;
    [SerializeField] private float distanceBuffer = 1.5f;
    [SerializeField] private float moveLockDuration = 1.0f;

    [Header("Selection")]
    public bool isSelected = false;

    // Visuals
    public SpriteRenderer spriteRenderer;
    private Animator animator;

    // Stats
    [Header("Starting Stats")]
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private int startingAttack = 5;
    [SerializeField] private int startingMagic = 5;
    [SerializeField] private int startingResist = 5;
    [SerializeField] private int sp = 0;

    // Movement
    [Header("View Only")]
    public bool isMoving = false;
    public bool movementLock = false;
    protected Player activePlayer;
    public GameObject target;
    private float lockTime = 0.0f;

    [Header("Visual Refs")]
    [SerializeField] private Image healthCircle;

    // Internal CD
    protected float internalNormalCD = 0.0f;
    protected float internalSkillCD = 0.0f;
    protected float internalEveCD = 0.0f;

    private Rigidbody2D rb;
    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        activePlayer = GetComponentInParent<Player>();

        health = startingHealth;
        attack = startingAttack;
        magic = startingMagic;
        resist = startingResist;
        speed = movementSpeed;

        currentHP = health;

        originalSpeed = speed;
    }

    protected override void Update()
    {
        // Death check
        if (state == ENTITY_STATE.dead) return;

        base.Update();

        if (currentHP <= 0) Die();

        // Cooldown timers
        if (internalNormalCD < 50f) internalNormalCD += Time.deltaTime;
        if (internalSkillCD < 50f) internalSkillCD += Time.deltaTime;
        if (internalEveCD < 50f) internalEveCD += Time.deltaTime;

        if (movementLock) {
            lockTime += Time.deltaTime;
            if (lockTime > moveLockDuration)
            {
                movementLock = false;
                lockTime = 0.0f;
            }
            return;
        } 

        if (!isSelected)
        {
            if (!activePlayer.inCombat)
            {
                target = activePlayer.GetActiveCharacter().gameObject;
            }
            else
            {
                FindEnemyTarget();
            }
            TrackActiveTarget();
            animator.SetBool("isMoving", isMoving);
            return;
        }

        isMoving = joystick.isDragging;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            spriteRenderer.flipX = joystick.Direction.x > 0 ? true : false;

            Vector3 newDir = joystick.GetDirAsVec3() * PersistentValues.speedMultiplier;
            rb.AddForce(newDir * speed * Time.deltaTime);
            //Vector3 step = transform.position + (joystick.GetDirAsVec3() * speed * Time.deltaTime);
            //rb.MovePosition(step);
            //transform.position += joystick.GetDirAsVec3() * speed * Time.deltaTime;
        }
    }


    protected virtual void TrackActiveTarget() 
    {
        if (target == null) return;

        Vector3 newDir = Vector3.zero;
        Vector3 joystickDir = new Vector3(joystick.Direction.x, joystick.Direction.y, 0.0f);
        Vector3 trackPos = target.transform.position + (joystick.GetDirAsVec3());

        newDir = trackPos - this.transform.position;
        newDir.Normalize();

        spriteRenderer.flipX = newDir.x > 0 ? true : false;

        float dist = activePlayer.inCombat ? enemyTrackDistance : trackDistance;
        float distanceToTarget = Vector2.Distance(trackPos, this.transform.position);
        Vector3 minDistance = transform.position - trackPos;

        if (distanceToTarget >= dist)
        {
            //transform.position += newDir * speed * Time.deltaTime;

            rb.AddForce(newDir * speed * Time.deltaTime * PersistentValues.speedMultiplier);
            isMoving = true;
        }
        else if (activePlayer.inCombat && distanceToTarget < enemyTrackDistance - distanceBuffer ||
            !activePlayer.inCombat && distanceToTarget < trackDistance/2 - distanceBuffer)
        {
            newDir *= -1;
            //transform.position += newDir * speed * Time.deltaTime;
            rb.AddForce(newDir * speed * Time.deltaTime * PersistentValues.speedMultiplier);
            isMoving = true;
        }
        else if (!joystick.isDragging && !activePlayer.inCombat)
        {
            isMoving = false;
            spriteRenderer.flipX = activePlayer.GetActiveIsFlipped();
        }
    }

    public bool GetFlipped()
    {
        return spriteRenderer.flipX;
    }

    // COMBAT -----------------------------------------------------------------------
    public List<Collider2D> enemiesInRange = new List<Collider2D>();
    void FixedUpdate()
    {
        enemiesInRange.RemoveAll(x => x.gameObject.GetComponent<GameEntity>().state == ENTITY_STATE.dead);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!enemiesInRange.Contains(collision) && collision.gameObject.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (collision.gameObject.GetComponent<Enemy>().state != Enemy.ENTITY_STATE.dead) enemiesInRange.Add(collision);
        }
    }

    public virtual void FindEnemyTarget() { }
    public virtual void UseNormalAttack() {
        FindEnemyTarget();
        movementLock = true;

        ResetAnimTriggers();
        animator.SetTrigger("Normal");
    }
    public virtual void UseEve() 
    {
        FindEnemyTarget();
        movementLock = true;

        ResetAnimTriggers();
        animator.SetTrigger("Eve");
    }
    public virtual void UseSkill() {
        FindEnemyTarget();
        movementLock = true;

        ResetAnimTriggers();
        animator.SetTrigger("Skill");
    }

    public void StartCombat()
    {
        activePlayer.inCombat = true;
    }
    public void EndCombat()
    {
        activePlayer.inCombat = false;
    }

    protected virtual void Die()
    {
        spriteRenderer.color = Color.gray;
        //ResetAllTriggers();
        isMoving = false;
        animator.SetBool("isMoving", isMoving);
        target = null;

        state = ENTITY_STATE.dead;
        //animator.SetTrigger("Die");
    }

    public virtual void Revive()
    {
        spriteRenderer.color = Color.white;
        //ResetAllTriggers();

        state = ENTITY_STATE.idle;
        //animator.SetTrigger("Die");
    }

    // STAT MANIP -------------------------------------------------------------------
    public void LevelUp()
    {
        health = Mathf.FloorToInt(health * (1 + 0.1f));
        AlterSP(10);

        for (int i = 0; i < 5; i++)
        {
            int stat = Random.Range(0, 4);
            int raiseVal = Random.Range(1, 5);
            STATS target = (STATS)(stat);

            AlterStat(target, raiseVal);
        }
    }

    public override void AlterHP(int val)
    {
        base.AlterHP(val);
        healthCircle.fillAmount = currentHP / (float)health;
    }

   public void AlterSP(int val)
    {
        sp += val;
    }

    public override void ClearSpdDebuff() {
        speed = movementSpeed;
    }

    // Getters
    public float GetNormalCD()
    {
        return internalNormalCD;
    }
    public float GetSkillCD()
    {
        return internalSkillCD;
    }
    public float GetEveCD()
    {
        return internalEveCD;
    }

    // Other

    private void ResetAnimTriggers()
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }
}
