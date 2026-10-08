using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
}
