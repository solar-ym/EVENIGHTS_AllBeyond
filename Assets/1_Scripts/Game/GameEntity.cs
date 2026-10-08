using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    [Header("Stats | View Only")]
    [SerializeField] protected int health = 100;
    [SerializeField] protected int attack = 5;
    [SerializeField] protected int magic = 5;
    [SerializeField] protected int resist = 5;
    [SerializeField] protected float speed = 5;
    protected int currentHP = 100;

    // Stat Modifiers
    [HideInInspector] public int atkMod = 0;
    [HideInInspector] public int magMod = 0;
    [HideInInspector] public int resMod = 0;

    // Debuffs
    [HideInInspector] public bool poisoned = false;
    [HideInInspector] public bool resDebuffed = false;
    [HideInInspector] public bool spdDebuffed = false;

        // Timers
        private float poisonTimer = 0.0f;
        private float resDebuffTimer = 0.0f;
        private float spdDebufftimer = 0.0f;
        private float internalTime = 0.0f;

        private float maxPoisonDuration = 5.0f;
        private float maxResDebuffDuration = 10.0f;
        private float maxSpdDebuffDuration = 5.0f;

        // Stacks + Other Mods
        private float poisonModifier = 0.01f; // percentage of hp taken per second
        private int spdDebuffStacks = 0; // max 3

    private void Update()
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

        if (resDebuffed)
        {
            resDebuffTimer += Time.deltaTime;
            if (resDebuffTimer >= maxResDebuffDuration)
            {
                ClearResDebuff();
                resDebuffed = false;
                resDebuffTimer = 0.0f;
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

        if (internalTime >= 1) internalTime = 0;
    }

    public virtual void AlterHP(int val)
    {
        currentHP += val;
        currentHP = Mathf.Clamp(currentHP, 0, health);
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

    public enum STATS
    {
        atk = 0,
        mag,
        res,
        maxHp
    };

    // Debuffs
    public virtual void Poison(float newMod)
    {
        if (newMod > poisonModifier) poisonModifier = newMod;
        poisoned = true;

        poisonTimer = 0.0f;
    }

    public virtual void DebuffRes(float percentage)
    {
        if (resDebuffed) ClearResDebuff();
        
        ModRes(percentage * -1);
        resDebuffed = true;
        resDebuffTimer = 0.0f;
    }

    public virtual void DebuffSpd()
    {
        if (spdDebuffStacks == 3) return;

        spdDebuffStacks = Mathf.Clamp(spdDebuffStacks + 1, 0, 3);

        speed = speed * (0.05f * spdDebuffStacks);

        spdDebuffed = true;
        spdDebufftimer = 0.0f;
    }

    public virtual void ClearResDebuff() { }
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

    public virtual void ModAtk(float percentage)
    {
        atkMod += Mathf.FloorToInt(attack * percentage);
    }

    public virtual void ModMag(float percentage)
    {
        magMod += Mathf.FloorToInt(magic * percentage);
    }

    public virtual void ModRes(float percentage)
    {
        resMod += Mathf.FloorToInt(resist * percentage);
    }
}
