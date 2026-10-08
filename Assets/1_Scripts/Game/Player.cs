using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GameEntity;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TextMeshProUGUI xpText;

    [Header("Config")]
    [SerializeField] private float maxCharDistance = 20.0f;
    [SerializeField] private bool keyboard = true;

    private List<Character> characters = new List<Character>();
    private int activeCharIndex = 0;
    public bool inCombat = false;

    // Player Stats
    private int level = 1;
    private int experiencePts = 0;
    private int nextExpCap = 100;

    private List<FLAT_BUFF> buffList = new List<FLAT_BUFF>();

    void Start()
    {
        characters.AddRange(GetComponentsInChildren<Character>());   
    }

    private void Update()
    {
        xpBarFill.fillAmount = experiencePts / (float)nextExpCap;

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

        PollBuffs();

        if (keyboard)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                characters[activeCharIndex].FindEnemyTarget();
                characters[activeCharIndex].UseNormalAttack();
            }
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
