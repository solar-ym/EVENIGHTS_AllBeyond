using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject spawnPoint;
    [SerializeField] float maxFollowDistance = 10.0f;
    [SerializeField] float trackDistance = 3.0f;

    [Header("Stat Ranges")]
    [SerializeField] int minHP = 10;
    [SerializeField] int maxHP = 20;
    [SerializeField] int minAtk = 10;
    [SerializeField] int maxAtk = 15;
    [SerializeField] int minRes = 0;
    [SerializeField] int maxRes = 10;
    [SerializeField] float minSpd = 2;
    [SerializeField] float maxSpd = 5;

    private int currentHP = 0;
    private int health = 0;
    private int attack = 0;
    private int resist = 0;
    private float speed = 0;

    ENEMY_STATE state = ENEMY_STATE.idle;
    private Character target;

    private SpriteRenderer spriteRenderer;
    private bool isMoving = false;
    private bool returningToSpawn = false;

    public enum ENEMY_STATE
    {
        idle,
        attacking
    }

    void Start()
    {
        health = Random.Range(minHP, maxHP);
        attack = Random.Range(minAtk, maxAtk);
        resist = Random.Range(minRes, maxRes);
        speed = Random.Range(minSpd, maxSpd);

        currentHP = health;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!WithinSpawnRange() || returningToSpawn)
        {
            state = ENEMY_STATE.idle;
            ReturnToSpawn();
        }

        if (state == ENEMY_STATE.idle && WithinSpawnRange()) return;

        else if (state == ENEMY_STATE.attacking && WithinSpawnRange())
        {
            TrackActiveTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Character")
        {
            state = ENEMY_STATE.attacking;
            target = collision.gameObject.GetComponent<Character>();
        }
    }

    protected virtual void TrackActiveTarget()
    {
        Vector3 newDir = Vector3.zero;
        Vector3 trackPos = target.transform.position;

        newDir = trackPos - this.transform.position;
        newDir.Normalize();

        spriteRenderer.flipX = newDir.x > 0 ? true : false;

        if (Vector2.Distance(trackPos, this.transform.position) >= trackDistance)
        {
            transform.position += newDir * speed * Time.deltaTime;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
    protected virtual void ReturnToSpawn()
    {
        Vector3 newDir = Vector3.zero;
        Vector3 trackPos = spawnPoint.transform.position;

        newDir = trackPos - this.transform.position;
        newDir.Normalize();

        spriteRenderer.flipX = newDir.x > 0 ? true : false;

        if (Vector2.Distance(trackPos, this.transform.position) >= 1)
        {
            returningToSpawn = true;
            transform.position += newDir * speed * Time.deltaTime;
            isMoving = true;
        }
        else
        {
            returningToSpawn = false;
            isMoving = false;
        }
    }

    private bool WithinSpawnRange()
    {
        return (Vector3.Distance(this.transform.position, spawnPoint.transform.position) <= maxFollowDistance);
    }

    // COMBAT -----------------------------------------------------------------------
    protected virtual void UseNormalAttack() { }
    protected virtual void UseEve() { }
    protected virtual void UseSkill() { }
}
