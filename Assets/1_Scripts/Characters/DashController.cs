using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static SkillCDVisual;

public class DashController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player activePlayer;

    [Header("Visuals")]
    [SerializeField] private Image dashCooldownFill;

    private Vector2 startPos = new Vector2(0, 0);
    private Vector2 endPos = new Vector2(0, 0);

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

    private void CheckSwipe()
    {
        if (activePlayer.internalDashCD < dashCooldown) return;

        Vector3 directionRegistered = endPos - startPos;

        activePlayer.GetActiveCharacter().Dash(directionRegistered);
        activePlayer.internalDashCD = 0.0f;
        dashCooldownFill.fillAmount = 0.0f;
    }

    public void ResetDash()
    {
        activePlayer.internalDashCD = 0.0f;
        dashCooldownFill.fillAmount = 0.0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Tap registered");
        startPos = Pointer.current.position.ReadValue();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        endPos = Pointer.current.position.ReadValue();
        CheckSwipe();
    }
}
