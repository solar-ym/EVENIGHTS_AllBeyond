using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] FixedJoystick joystick;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float trackDistance = 1.5f;

    [Header("Selection")]
    public bool isSelected = false;

    // Visuals
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Movement
    public bool isMoving = false;
    private Player activePlayer;
    private GameObject target;

    // Stats
    [Header("Stats")]
    [SerializeField] private int health = 100;
    [SerializeField] private int attack = 5;
    [SerializeField] private int magic = 5;
    [SerializeField] private int resist = 5;
    [SerializeField] private int sp = 0;
    private int currentHP = 100;

    [Header("Visual Refs")]
    [SerializeField] private Image healthCircle; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        activePlayer = GetComponentInParent<Player>();
        currentHP = health;
    }

    protected virtual void Update()
    {
        if (!isSelected)
        {
            if (!activePlayer.inCombat) target = activePlayer.GetActiveCharacter().gameObject;
            TrackActiveTarget();
            animator.SetBool("isMoving", isMoving);
            return;
        }

        isMoving = joystick.isDragging;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            spriteRenderer.flipX = joystick.Direction.x > 0 ? true : false;

            transform.position += joystick.GetDirAsVec3() * speed * Time.deltaTime; 
        }
    }

    protected virtual void TrackActiveTarget() 
    {
        Vector3 newDir = Vector3.zero;
        Vector3 joystickDir = new Vector3(joystick.Direction.x, joystick.Direction.y, 0.0f);
        Vector3 trackPos = target.transform.position + (joystick.GetDirAsVec3());

        newDir = trackPos - this.transform.position;
        newDir.Normalize();

        spriteRenderer.flipX = newDir.x > 0 ? true : false;

        if (Vector2.Distance(trackPos, this.transform.position) >= trackDistance)
        {
            transform.position += newDir * speed * Time.deltaTime;
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
        enemiesInRange.Clear();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!enemiesInRange.Contains(collision.collider))
        {
            enemiesInRange.Add(collision.collider);
        }
    }
    protected virtual GameObject FindEnemyTarget() { return new GameObject(); }
    public virtual void UseNormalAttack() { }
    public virtual void UseEve() { }
    public virtual void UseSkill() { }

    // STAT MANIP -------------------------------------------------------------------
    public void LevelUp()
    {
        health = Mathf.FloorToInt(health * (1 + 0.1f));
        sp += 10;

        for (int i = 0; i < 5; i++)
        {
            int stat = Random.Range(0, 4);
            int raiseVal = Random.Range(1, 5);
            CHAR_STATS target = (CHAR_STATS)(stat);

            AlterStat(target, raiseVal);
        }
    }
    public void AlterHP(int val)
    {
        currentHP += val;
        currentHP = Mathf.Clamp(currentHP, 0, health);   

        healthCircle.fillAmount = currentHP / (float)health;
    }

    public void AlterStat(CHAR_STATS stat, int val)
    {
        switch (stat)
        {
            case CHAR_STATS.atk:
                attack += val;
                break;
            case CHAR_STATS.mag:
                magic += val;
                break;
            case CHAR_STATS.res:
                resist += val;
                break;
            case CHAR_STATS.maxHp:
                health += val;
                break;
            default:
                if (GameDebugger.debugMode) Debug.Log("Alter stat random miss.");
                break;
        } 
    }

    public void AlterSP(int val)
    {
        sp += val;
    }

    public enum CHAR_STATS
    {
        atk = 0,
        mag,
        res,
        maxHp
    };
}
