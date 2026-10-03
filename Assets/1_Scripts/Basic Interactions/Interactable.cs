using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class Interactable : MonoBehaviour
{
    [Header("Actions/Events")]
    [SerializeField] private List<UnityEvent> functionToCall = new List<UnityEvent>();
    [SerializeField] private List<GameEvent> eventToTrigger = new List<GameEvent>();

    public void TriggerFunction()
    {
        foreach (UnityEvent func in functionToCall)
        {
            func.Invoke();
        }
    }

    public void TriggerEvent()
    {
        foreach (GameEvent gameEvent in eventToTrigger)
        {
            gameEvent.TriggerEvent();
        }
    }

    public void TriggerAll()
    {
        if (eventToTrigger != null && eventToTrigger.Count > 0) TriggerEvent();
        if (functionToCall != null && functionToCall.Count > 0) TriggerFunction();
    }
}
