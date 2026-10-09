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

    public float timer = 0.0f;
    public bool running = true;

    public void RestartTimer()
    {
        if (running) return;

        fill.fillAmount = 1;
        timer = 0.0f;
        fill.raycastTarget = true;

        running = true;
    }
}
