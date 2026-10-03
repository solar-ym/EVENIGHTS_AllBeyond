using UnityEngine;
using UnityEngine.Events;

public class ViewableObj : Interactable
{
    [Header("Note: Can only be triggered once, UNLESS [repeatable] is true.\n[timeOutSeconds] is how many seconds until object can be triggered again.")]
    [Header("Note 2: if [triggerOnLeave] is true, the attached actions/events will trigger\nwhen the object leaves the screen instead of when it enters (not both).")]

    [Header("Config")]
    public bool triggerOnLeave = false;
    public bool repeatable = true;
    [SerializeField] private float timeOutSeconds = 1.0f;

    protected bool hasBeenActivated = false;
    private bool hasBeenSeen = false;
    private float time = 0.0f;

    public virtual void Update()
    {

        if (hasBeenActivated && !IsWithinViewPort() && repeatable)
        {
            time += Time.deltaTime;
            if (time > timeOutSeconds)
            {
                TimeOutReset();
                time = 0.0f;    
            }
        }

        if (IsWithinViewPort())
        {
            hasBeenSeen = true;

            if (!hasBeenActivated && !triggerOnLeave)
            {
                TriggerAll();
                hasBeenActivated = true;
            }
        }
        else if (!hasBeenActivated && hasBeenSeen && triggerOnLeave && !IsWithinViewPort())
        {
            TriggerAll();
            hasBeenActivated = true;
        }
    }

    protected bool IsWithinViewPort()
    {
        Vector3 viewPortPos = Camera.main.WorldToViewportPoint(transform.position);

        return (viewPortPos.x >= 0 && viewPortPos.x <= 1 &&
               viewPortPos.y >= 0 && viewPortPos.y <= 1 
            );
    }

    private void TimeOutReset()
    {
        hasBeenActivated = false;
        hasBeenSeen = false;
    }
}
