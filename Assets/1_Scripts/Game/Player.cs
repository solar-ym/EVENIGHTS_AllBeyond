using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TextMeshProUGUI xpText;

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
            if (i == index) characters[i].isSelected = true;
            else characters[i].isSelected = false;
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
