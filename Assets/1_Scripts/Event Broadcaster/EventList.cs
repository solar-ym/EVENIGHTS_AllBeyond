using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;


public static class EventList
{
    public static Dictionary<String, GameEvent> allGameEvents = new Dictionary<String, GameEvent>(); 

    public static void TriggerEvent(String eventName)
    {
        allGameEvents[eventName].TriggerEvent();
    }
}
