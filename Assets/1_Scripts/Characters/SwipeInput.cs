using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class SwipeInput : Interactable, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [Tooltip("Should just be any object that's in the direction you want the swipe to go in (relative to this object).")]
    [SerializeField] protected GameObject directionReference;
    [SerializeField] protected GameObject startReference;

    [Header("Config")]
    [Tooltip("Should be a value over 0 and <= 1, where 1 means the direction has to be perfectly exact.")]
    [SerializeField] protected float swipeLeeway = 0.8f;
    [SerializeField] protected bool preventMultipleActivations = true;

    protected Vector2 startPos = new Vector2 (0, 0);
    protected Vector2 endPos = new Vector2 (0, 0);

    protected Vector3 direction = new Vector3(0, 0, 0);

    [Header("For Observation")]
    [SerializeField] protected bool hasBeenActivated = false;

    private void Start()
    {
        direction = directionReference.transform.position - startReference.transform.position;
        direction.Normalize();
    }

    protected virtual void CheckSwipe()
    {
        if (hasBeenActivated && preventMultipleActivations) return;

        Vector3 directionRegistered = endPos - startPos;
        directionRegistered.Normalize();

        float dot = Vector3.Dot(direction, directionRegistered);

        if (dot >= swipeLeeway)
        {
            TriggerAll();
            hasBeenActivated = true;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = Pointer.current.position.ReadValue();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        endPos = Pointer.current.position.ReadValue();
        CheckSwipe();
    }
}
