using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class EntropyBar : MonoBehaviour
{
    [SerializeField] private Volume chromaticAberration;
    [SerializeField] private Player player;

    private ChromaticAberration chroma;
    private Image barFill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        barFill = GetComponent<Image>();
    }

    private void Update()
    {
        AlterEntropyVisual();
    }

    public void AlterEntropyVisual()
    {
        if (barFill.fillAmount == player.entropy) return;

        barFill.fillAmount = player.entropy;

        barFill.fillAmount = Mathf.Clamp01(barFill.fillAmount);

        if (chromaticAberration.profile.TryGet<ChromaticAberration>(out chroma))
        {
            if (!player.inCombat && chroma.intensity.value > 0.0f)
            {
                chroma.intensity.value = Mathf.Clamp01(chroma.intensity.value - 0.01f);
            } else
            {
                chroma.intensity.value = Mathf.Clamp01(barFill.fillAmount / 2);
            }
        }
    }
}
