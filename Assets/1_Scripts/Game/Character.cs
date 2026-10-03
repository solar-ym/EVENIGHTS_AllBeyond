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

    void Update()
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
    protected virtual Vector3 FindEnemyTarget() { return new Vector3(); }
    protected virtual void UseNormalAttack() { }
    protected virtual void UseEve() { }
    protected virtual void UseSkill() { }

    // STAT MANIP -------------------------------------------------------------------

    public void AlterHP(int val)
    {
        currentHP += val;
        currentHP = Mathf.Clamp(currentHP, 0, health);   

        healthCircle.fillAmount = currentHP / health;
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
