using System.Collections.Generic;
using UnityEngine;
using static GameEntity;

public class GameEntity : MonoBehaviour
{
    [Header("Stats | View Only")]
    [SerializeField] public int health = 100;
    [SerializeField] public int attack = 5;
    [SerializeField] public int magic = 5;
    [SerializeField] public int resist = 5;
    [SerializeField] protected float speed = 5;
    public ENTITY_STATE state = ENTITY_STATE.idle;
    protected int currentHP = 100;

    // Stat Modifiers
    public int atkMod = 0;
    public int magMod = 0;
    public int resMod = 0;
    private List<BUFF> buffs = new List<BUFF>();

    // Debuffs
    [HideInInspector] public bool poisoned = false;
    [HideInInspector] public bool resDebuffed = false;
    [HideInInspector] public bool spdDebuffed = false;

        // Timers
        private float poisonTimer = 0.0f;
        private float spdDebufftimer = 0.0f;
        private float internalTime = 0.0f;

        private float maxPoisonDuration = 5.0f;
        private float maxSpdDebuffDuration = 5.0f;

        // Stacks + Other Mods
        private float poisonModifier = 0.01f; // percentage of hp taken per second
        private int spdDebuffStacks = 0; // max 3

    private DamageNumber numberDisplaySpawner;

    protected virtual void Start()
    {
        numberDisplaySpawner = GetComponentInChildren<DamageNumber>();
    }

    protected virtual void Update()
    {

        internalTime += Time.deltaTime;

        if (poisoned)
        {
            poisonTimer += Time.deltaTime;
            if (internalTime >= 1)
            {
                PoisonDamage();
            }
            if (poisonTimer >= maxPoisonDuration)
            {
                poisonModifier = 0;
                poisoned = false;
                poisonTimer = 0.0f;
            }
        }

        if (spdDebuffed)
        {
            spdDebufftimer += Time.deltaTime;
            if (spdDebufftimer >= maxSpdDebuffDuration)
            {
                ClearSpdDebuff();
                spdDebufftimer = 0.0f;
            }
        }

        for (int i = 0; i < buffs.Count; ++i)
        {
            BUFF thisBuff = buffs[i];
            thisBuff.life += Time.deltaTime;
           
            if (!thisBuff.hasActivated)
            {
                thisBuff.value = TriggerBuff(thisBuff);
                thisBuff.hasActivated = true;
            } else if (thisBuff.life >= thisBuff.lifeSpan)
            {
                ClearBuff(thisBuff);
            }

            buffs[i] = thisBuff;
        }

        buffs.RemoveAll(buff => buff.life >= buff.lifeSpan);

        if (internalTime >= 1) internalTime = 0;
    }

    public virtual void AlterHP(int val)
    {
        if (state == ENTITY_STATE.dead) return;

        currentHP += val;
        currentHP = Mathf.Clamp(currentHP, 0, health);

        if (val > 0)
        {
            numberDisplaySpawner.SpawnHealText(val);
        }
    }

    public void SpawnDamageNumber(DamageCalculator.DAMAGE_TYPE damageType, int damageValue)
    {
        if (numberDisplaySpawner == null) numberDisplaySpawner = GetComponentInChildren<DamageNumber>();

        numberDisplaySpawner.SpawnDamageNumber(damageType, damageValue);
    }

    public virtual void AlterStat(STATS stat, int val)
    {
        switch (stat)
        {
            case STATS.atk:
                attack += val;
                break;
            case STATS.mag:
                magic += val;
                break;
            case STATS.res:
                resist += val;
                break;
            case STATS.maxHp:
                health += val;
                break;
            default:
                if (GameDebugger.debugMode) Debug.Log("Alter stat random miss.");
                break;
        }
    }

    // Debuffs -------------------------------------------------------
    public virtual void Poison(float newMod)
    {
        if (newMod > poisonModifier) poisonModifier = newMod;
        poisoned = true;

        poisonTimer = 0.0f;
    }

    public virtual void DebuffSpd()
    {
        if (spdDebuffStacks == 3) return;

        spdDebuffStacks = Mathf.Clamp(spdDebuffStacks + 1, 0, 3);

        speed = speed * (0.05f * spdDebuffStacks);

        spdDebuffed = true;
        spdDebufftimer = 0.0f;
    }

    public virtual void ClearSpdDebuff() {
        spdDebuffStacks = Mathf.Clamp(spdDebuffStacks - 1, 0, 3);

        if (spdDebuffStacks == 0)
        {
            spdDebuffed = false;
            return;
        }

        speed = speed * (0.05f * spdDebuffStacks);
    }

    private void PoisonDamage()
    {
        int poisonDmg = Mathf.FloorToInt(poisonModifier * health * -1);
        AlterHP(poisonDmg);
    }

    // Stat Mod Manip -------------------------------------------------------

    public void AddBuff(STATS affectedStat, float modifier, int buffOrDebuff, float lifespan)
    {

        BUFF modToAdd = new BUFF(
                affectedStat,
                modifier * buffOrDebuff,
                lifespan
            );


        buffs.Add( modToAdd );
    }

    private int TriggerBuff(BUFF buff)
    {
        int val = 0;
        switch (buff.affectedStat)
        {
            case STATS.atk:
                val = ModAtk(buff.modifier);
                break;
            case STATS.mag:
                val = ModMag(buff.modifier);
                break;
            case STATS.res:
                val = ModRes(buff.modifier);
                break;
        }

        return val;
    }

    private void ClearBuff(BUFF buff)
    {
        FlatBuff(buff.affectedStat, buff.value * -1);
    }

    public void FlatBuff(STATS stat, int mod)
    {
        switch (stat)
        {
            case STATS.atk:
                atkMod += mod;
                break;
            case STATS.mag:
                magMod += mod;
                break;
            case STATS.res:
                resMod += mod;
                break;
        }
    }

    public virtual int ModAtk(float percentage)
    {
        int val = Mathf.FloorToInt(attack * percentage);
        atkMod += val;

        return val;
    }

    public virtual int ModMag(float percentage)
    {
        int val = Mathf.FloorToInt(magic * percentage);
        magMod += val;

        return val;
    }

    public virtual int ModRes(float percentage)
    {
        int val = Mathf.FloorToInt(resist * percentage);
        resMod += val;

        return val;
    }

    public void CleanseMods()
    {
        atkMod = 0;
        magMod = 0;
        resMod = 0;
    }

    // Enums/Structs -----------------------------------------------------------
    public enum STATS
    {
        atk = 0,
        mag,
        res,
        maxHp
    };

    public struct BUFF
    {
        public BUFF(STATS stat, float mod, float span)
        {
            affectedStat = stat;
            modifier = mod;
            value = 0;
            lifeSpan = span;
            life = 0.0f;
            hasActivated = false;
        }
        public STATS affectedStat;
        public float modifier;
        public int value;
        public float lifeSpan;
        public float life;
        public bool hasActivated;
    };

    public enum ENTITY_STATE
    {
        idle,
        attacking,
        dead
    }
}
