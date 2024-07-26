using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EventManager
{
    private class EventInfo
    {
        public string evt;
        public object[] args;
    }

    private static EventManager instance = new EventManager();
    public static EventManager Instance => instance;

    private Dictionary<string, Action<object[]>> events = new Dictionary<string, Action<object[]>>();

    public static void Listen(string evt, Action<object[]> cb)
    {
        if (!instance.events.ContainsKey(evt))
        {
            instance.events[evt] = cb;
            return;
        }

        Dictionary<string, Action<object[]>> dictionary = instance.events;
        string key = evt;
        dictionary[key] = (Action<object[]>)Delegate.Remove(dictionary[key], cb);
        dictionary = instance.events;
        key = evt;
        dictionary[key] = (Action<object[]>)Delegate.Combine(dictionary[key], cb);
    }

    public static void Ignore(string evt, Action<object[]> cb)
    {
        if (instance.events.ContainsKey(evt))
        {
            Dictionary<string, Action<object[]>> dictionary = instance.events;
            dictionary[evt] = (Action<object[]>)Delegate.Remove(dictionary[evt], cb);
        }
    }

    public static void Trigger(string evt, params object[] args)
    {
        if (!instance.events.TryGetValue(evt, out var value) || value == null)
        {
            return;
        }

        value(args);
    }
}
