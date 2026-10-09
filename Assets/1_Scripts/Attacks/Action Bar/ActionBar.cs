using UnityEngine;
using System.Collections.Generic;

public class ActionBar : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player player;

    [Header("Skills")]
    [SerializeField] private GameObject medaSkills;
    [SerializeField] private GameObject shaiSkills;
    [SerializeField] private GameObject claureSkills;

    // Skill CD visuals
    private List<SkillCDVisual> medaSkillVisuals = new List<SkillCDVisual>();
    private List<SkillCDVisual> shaiSkillVisuals = new List<SkillCDVisual>();
    private List<SkillCDVisual> claureSkillVisuals = new List<SkillCDVisual>();

    private void Start()
    {
        medaSkillVisuals.AddRange(medaSkills.GetComponentsInChildren<SkillCDVisual>());
        shaiSkillVisuals.AddRange(shaiSkills.GetComponentsInChildren<SkillCDVisual>());
        claureSkillVisuals.AddRange(claureSkills.GetComponentsInChildren<SkillCDVisual>());
    }

    private void Update()
    {
        IterateSkillVisuals(medaSkillVisuals);
        IterateSkillVisuals(shaiSkillVisuals);
        IterateSkillVisuals(claureSkillVisuals);
    }

    private void IterateSkillVisuals(List<SkillCDVisual> visuals)
    {
        foreach (var skill in visuals)
        {
            if (!skill.running) continue;

            skill.timer += Time.deltaTime;

            if (skill.timer >= skill.cooldown)
            {
                skill.timer = 0;
                skill.running = false;

                continue;
            }

            skill.fill.fillAmount = 1 - (skill.timer / skill.cooldown);
        }
    }
}
