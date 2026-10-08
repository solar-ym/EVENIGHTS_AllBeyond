using UnityEngine;
using UnityEngine.UI;

public class SkillCDVisual : MonoBehaviour
{
    public enum SKILL
    {
        na,
        skill,
        eve
    };

    [Header("Config")]
    public Image fill;
    public float cooldown = 3.0f;
    public Character attachedChar;
    public SKILL skillType;

    private float timer = 0.0f;
    private bool running = true;

    private void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;

            fill.fillAmount -= (1 / cooldown) * Time.deltaTime;
            if (timer > cooldown)
            {
                fill.fillAmount = 0.0f;
                timer = 0.0f;
                fill.raycastTarget = false;

                running = false;
            }
        }
    }

    public void RestartTimer()
    {
        if (running) return;

        fill.fillAmount = 1;
        timer = 0.0f;
        fill.raycastTarget = true;

        running = true;
    }

    private void OnEnable()
    {
        float val = 0.0f;
        switch (skillType)
        {
            case SKILL.na:
                val = attachedChar.GetNormalCD();
                break;
            case SKILL.skill:
                val = attachedChar.GetSkillCD();
                break;
            case SKILL.eve:
                val = attachedChar.GetEveCD();
                break;
        }

        val = Mathf.Clamp(val, 0.0f, cooldown);
        timer = val;
        fill.fillAmount = Mathf.Clamp(timer / cooldown, 0f, 1f);

        running = true;

        if (timer >= cooldown)
        {
            fill.fillAmount = 0.0f;
            timer = 0.0f;
            fill.raycastTarget = false;

            running = false;
        }
    }
}
