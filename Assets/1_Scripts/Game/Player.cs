using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GameEntity;
using static UnityEngine.Rendering.DebugUI;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private GameObject deathScreen;

    [Header("Config")]
    [SerializeField] private float maxCharDistance = 20.0f;
    [SerializeField] private bool keyboard = true;
    [SerializeField] public float entropyGain = 0.01f;
    public float dashCooldown = 3.0f;

    private List<Character> characters = new List<Character>();
    private int activeCharIndex = 0;
    public bool inCombat = false;

    // Player Stats
    private int level = 1;
    private int experiencePts = 0;
    private int nextExpCap = 100;
    // Entropy 
    public float entropy = 0.0f;

    private List<FLAT_BUFF> buffList = new List<FLAT_BUFF>();

    // Dash
    [HideInInspector] public float internalDashCD = 0.0f;
    [HideInInspector] public bool dashed = false;

    void Start()
    {
        characters.AddRange(GetComponentsInChildren<Character>());
        PersistentValues.isKeyboard = keyboard;
        Debug.Log("keyboard" + PersistentValues.isKeyboard);
    }

    private void Update()
    {
        CheckIfAllDead();

        xpBarFill.fillAmount = experiencePts / (float)nextExpCap;

        PollBuffs();
        if (PersistentValues.isKeyboard) PollKeyboardInputs();

        if (!inCombat) return;
        
        int enemyCount = 0;

        // Detect combat
        foreach (Character character in characters)
        {
            if (character.enemiesInRange.Count == 0)
            {
                enemyCount++;
                if (enemyCount >= 2)
                {
                    inCombat = false; 
                    break;
                }
            }

            if (Vector3.Distance(characters[activeCharIndex].transform.position, character.transform.position) >= maxCharDistance)
            {
                inCombat = false; break;
            } 
        }
    }

    private void PollKeyboardInputs()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            characters[activeCharIndex].FindEnemyTarget();
            characters[activeCharIndex].UseNormalAttack();
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            characters[activeCharIndex].FindEnemyTarget();
            characters[activeCharIndex].UseSkill();
        }
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            characters[activeCharIndex].FindEnemyTarget();
            characters[activeCharIndex].UseEve();
        }
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            if (internalDashCD >= dashCooldown)
            {
                GetActiveCharacter().Dash(GetActiveCharacter().joystick.GetDirAsVec3());
                dashed = true;
            }
        }
    }

    private void CheckIfAllDead()
    {
        int i = 0;
        foreach (Character character in characters)
        {
            if (character.state == ENTITY_STATE.dead)
            {
                i++;
            }
        }

        if (i == 3)
        {
            Time.timeScale = 0;
            deathScreen.SetActive(true);
        }
    }

    // XP GAIN ---------------------------------------------------------------
    public void GainXP(int val)
    {
        experiencePts += val;
        if (experiencePts >= nextExpCap)
        {
            nextExpCap = Mathf.FloorToInt(nextExpCap * (1 + 0.3f));
            LevelUp();
        }
    }

    private void LevelUp()
    {
        if (level < 99) level++;
        PersistentValues.playerLevel = level;

        foreach (Character c in characters)
        {
            c.LevelUp();
        }

        xpText.text = level.ToString();
        experiencePts = 0;
    }


    public void SetActiveCharacter(int index)
    {
        activeCharIndex = index;
        for (int i = 0; i < characters.Count; i++)
        {
            if (i == index)
            {
                characters[i].isSelected = true;
            }
            else
            {
                characters[i].isSelected = false;
            }
        }
    }

    public void AddEntropy()
    {
        entropy = Mathf.Clamp01(entropy + entropyGain);
    }

    public void DrainEntropy()
    {
        entropy = 0;
    }

    public Character GetActiveCharacter()
    {
        return characters[activeCharIndex];
    }

    public bool GetIsActiveMoving()
    {
        return characters[activeCharIndex].isMoving;
    }

    public bool GetActiveIsFlipped()
    {
        return characters[activeCharIndex].GetFlipped();
    }

    public void HealAllCharacters(int value)
    {
        foreach (Character c in characters)
        {
            c.AlterHP(value);
        }
    }
    public virtual void ReviveAllCharacters()
    {
        foreach (Character c in characters)
        {
            c.Revive();
        }
    }

    // BUFFS ----------------------------------------------------

    private void PollBuffs()
    {
        for (int i = 0; i < buffList.Count; ++i)
        {
            FLAT_BUFF thisBuff = buffList[i];
            thisBuff.life += Time.deltaTime;

            if (!thisBuff.hasBeenTriggered)
            {
                foreach (Character character in characters)
                {
                    character.FlatBuff(thisBuff.stat, thisBuff.value);
                }
                thisBuff.hasBeenTriggered = true;
            }

            if (thisBuff.life >= thisBuff.lifeSpan)
            {
                foreach (Character character in characters)
                {
                    character.FlatBuff(thisBuff.stat, thisBuff.value * -1);
                }
            }

            buffList[i] = thisBuff;
        }

        buffList.RemoveAll(buff => buff.life >= buff.lifeSpan);
    }

    public void BuffAllCharacters(GameEntity.STATS stat, int modifier, float lifespan)
    {
        buffList.Add( new FLAT_BUFF(stat, modifier, lifespan) );
    }

    public struct FLAT_BUFF
    {
        public FLAT_BUFF(GameEntity.STATS targetStat, int val, float lifespan)
        {
            stat = targetStat;
            value = val;
            lifeSpan = lifespan;
            life = 0.0f;
            hasBeenTriggered = false;
        }
        public GameEntity.STATS stat;
        public int value;
        public float life;
        public float lifeSpan;
        public bool hasBeenTriggered;
    }
}
