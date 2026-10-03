using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class TappableObj : Interactable, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Config")]
    public bool triggerFunc = false;
    public bool triggerEvent = false;
    [SerializeField] private bool preventMultipleActivations;

    private bool hasBeenActivated = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hasBeenActivated && preventMultipleActivations) return;

        if (!triggerFunc) {
            TriggerEvent();
            return;
        }
        if (!triggerEvent)
        {
            TriggerFunction();
            return;
        }

        TriggerAll();

        hasBeenActivated = true;
    }

    // Empty functions required for IPointerClickHandler 
    public void OnPointerDown(PointerEventData eventData) {
        if (GameDebugger.debugMode) Debug.Log("Object: \"" + gameObject.name + "\" tap was registered.");
    }
    public void OnPointerUp(PointerEventData eventData) { }
}
