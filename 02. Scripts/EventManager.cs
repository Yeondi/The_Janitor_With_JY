using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private static Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

    public static void StartListening(string eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, object param)
    {
        if (eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent.Invoke(param);
        }
    }
}
