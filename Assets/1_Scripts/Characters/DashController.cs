using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static SkillCDVisual;

public class DashController : MonoBehaviour
{
    public Player activePlayer;

    [Header("Visuals")]
    [SerializeField] private Image dashCooldownFill;

    [HideInInspector] public float dashCooldown = 3.0f;

    private void Start()
    {
        dashCooldown = activePlayer.dashCooldown;
    }

    private void Update()
    {
        if (PersistentValues.isKeyboard && activePlayer.dashed)
        {
            ResetDash();
            activePlayer.dashed = false;
        }

        if (activePlayer.internalDashCD < 50f) activePlayer.internalDashCD += Time.deltaTime;

        if (activePlayer.internalDashCD >= dashCooldown)
        {
            dashCooldownFill.fillAmount = 1;
        }

        dashCooldownFill.fillAmount = Mathf.Clamp01(activePlayer.internalDashCD / dashCooldown);
    }

    public void TriggerDash()
    {
        if (activePlayer.internalDashCD < dashCooldown) return;

        activePlayer.GetActiveCharacter().Dash();
        activePlayer.internalDashCD = 0.0f;
        dashCooldownFill.fillAmount = 0.0f;
    }

    public void ResetDash()
    {
        activePlayer.internalDashCD = 0.0f;
        dashCooldownFill.fillAmount = 0.0f;
    }

}
