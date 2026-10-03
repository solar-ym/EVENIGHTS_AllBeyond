using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    private List<Character> characters = new List<Character>();

    private int activeCharIndex = 0;
    public bool inCombat = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characters.AddRange(GetComponentsInChildren<Character>());   
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
