using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/GameEvent")]
public class GameEvent : ScriptableObject
{
    private List<EventListener> allListeners = new List<EventListener>();

    private void OnEnable()
    {
        EventList.allGameEvents.Add(this.name, this);
    }

    public void TriggerEvent()
    {
        foreach (var listener in allListeners)
        {
            listener.EventTriggered();
        }
    }

    public void AddListener(EventListener newListener)
    {
        allListeners.Add(newListener);
    }

    public void RemoveListener(EventListener listenerToRemove)
    {
        allListeners.Remove(listenerToRemove);
    }

    private void OnDisable()
    {
        EventList.allGameEvents.Remove(this.name); 
    }
}
