using UnityEngine;
using UnityEngine.UI;

public class SkillCDVisual : MonoBehaviour
{

    [Header("Config")]
    public Image fill;
    public float cooldown = 3.0f;

    private float timer = 0.0f;
    private bool running = true;

    private void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;

            fill.fillAmount -= (1 / (float)3) * Time.deltaTime;
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
}
