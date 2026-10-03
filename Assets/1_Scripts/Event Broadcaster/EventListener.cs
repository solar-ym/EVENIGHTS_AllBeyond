using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    public GameEvent gameEvent; // What event it should be listening to
    public UnityEvent funcToCall; // What it should call when the event occurs

    private void OnEnable()
    {
        gameEvent.AddListener(this);
    }

    private void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }

    public void EventTriggered()
    {
        funcToCall.Invoke();
    }
}
