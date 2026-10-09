using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Character;

public class Enemy : GameEntity
{
    [Header("References")]
    [SerializeField] public GameObject spawnPoint;
    [SerializeField] Image healthBarFill;
    [SerializeField] float maxFollowDistance = 10.0f;
    [SerializeField] float trackDistance = 3.0f;
    [SerializeField] bool despawnAfterDeath = false;
    [SerializeField] float deathDespawn = 10.0f;

    [Header("Attacks")]
    [SerializeField] AttackSpawner normalAttack;

    [Header("Attack Cooldowns")]
    [SerializeField] private float normalCD = 0.5f;

    [Header("Stat Ranges")]
    [SerializeField] int minHP = 10;
    [SerializeField] int maxHP = 20;
    [SerializeField] int minAtk = 10;
    [SerializeField] int maxAtk = 15;
    [SerializeField] int minRes = 0;
    [SerializeField] int maxRes = 10;
    [SerializeField] float minSpd = 2;
    [SerializeField] float maxSpd = 5;

    private Character target;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isMoving = false;
    private bool returningToSpawn = false;

    // Internal CD
    private float internalNormalCD = 0.0f;
    private float internalSkillCD = 0.0f;
    private float internalEveCD = 0.0f;

    private float time = 0.0f;

    protected override void Start()
    {
        base.Start();

        health = Random.Range(minHP, maxHP);
        attack = Random.Range(minAtk, maxAtk);
        resist = Random.Range(minRes, maxRes);
        speed = Random.Range(minSpd, maxSpd);

        currentHP = health;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (state == ENTITY_STATE.dead) {
            if (despawnAfterDeath)
            {
                time += Time.deltaTime;
                if (time >= deathDespawn) Destroy(this.gameObject);
            }
            return;
        };

        animator.SetBool("isMoving", isMoving);

        if (!WithinSpawnRange() || returningToSpawn)
        {
            state = ENTITY_STATE.idle;
            ReturnToSpawn();
        }

        if (state == ENTITY_STATE.idle && WithinSpawnRange()) return;

        else if (state == ENTITY_STATE.attacking && WithinSpawnRange())
        {
            internalNormalCD += Time.deltaTime;

            TrackActiveTarget();
            if (internalNormalCD >= normalCD)
            {
                UseNormalAttack();
                internalNormalCD = 0.0f;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (state == ENTITY_STATE.dead) return;

        if (collision.gameObject.tag == "Character" && !collision.isTrigger)
        {
            state = ENTITY_STATE.attacking;
            target = collision.gameObject.GetComponent<Character>();
            collision.gameObject.GetComponent<Character>().StartCombat();
        }
    }

    protected virtual void TrackActiveTarget()
    {
        Vector3 newDir = Vector3.zero;
        Vector3 trackPos = target.transform.position;

        newDir = trackPos - this.transform.position;
        newDir.Normalize();

        spriteRenderer.flipX = newDir.x > 0 ? true : false;

        float distanceToTarget = Vector2.Distance(trackPos, this.transform.position);

        if (distanceToTarget >= trackDistance)
        {
            transform.position += newDir * speed * Time.deltaTime;
            isMoving = true;
        }
        else if (distanceToTarget < trackDistance - 1.5f)
        {
            newDir *= -1;
            transform.position += newDir * speed * Time.deltaTime;
            isMoving = true;
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

    protected virtual void UseNormalAttack() {
        if (state != ENTITY_STATE.attacking || target == null) return;

        ResetAllTriggers();
        animator.SetTrigger("Normal");

        Vector3 dir = Vector3.zero;
        Vector3 trackPos = target.transform.position;

        dir = trackPos - this.transform.position;
        dir.Normalize();

        normalAttack.SpawnAttack(dir);
    }
    protected virtual void UseEve() { }
    protected virtual void UseSkill() { }

    protected virtual void Die()
    {
        spriteRenderer.color = Color.gray;
        ResetAllTriggers();
        isMoving = false;
        target = null;

        state = ENTITY_STATE.dead;
        animator.SetTrigger("Die");

        spriteRenderer.sortingLayerName = "Enemy Behind";

        EventList.TriggerEvent("AddExp");
    }

    // ANIMATIONS/VISUALS

    protected virtual void ResetAllTriggers()
    {
        animator.ResetTrigger("Normal");
    }

    // STAT MANIP -------------------------------------------------------------------
    public override void AlterHP(int val)
    {
        base.AlterHP(val);

        healthBarFill.fillAmount = currentHP / (float)health;
        if (currentHP <= 0) Die();
    }
}
