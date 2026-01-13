using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionScript
{
    /// <summary>
    /// Mimics Flash EventDispatcher for event handling.
    /// </summary>
    public class EventDispatcher
    {
        private readonly Dictionary<string, List<Action<Event>>> _eventListeners = new();

        public void AddEventListener(string type, Action<Event> listener)
        {
            if (!_eventListeners.ContainsKey(type))
                _eventListeners[type] = new List<Action<Event>>();
            
            if (!_eventListeners[type].Contains(listener))
                _eventListeners[type].Add(listener);
        }

        public void RemoveEventListener(string type, Action<Event> listener)
        {
            if (_eventListeners.ContainsKey(type))
                _eventListeners[type].Remove(listener);
        }

        public bool HasEventListener(string type)
        {
            return _eventListeners.ContainsKey(type) && _eventListeners[type].Count > 0;
        }

        public bool DispatchEvent(Event evt)
        {
            string type = evt.Type;
            if (!_eventListeners.ContainsKey(type))
                return false;
            
            // Copy list to allow modification during iteration
            var listeners = new List<Action<Event>>(_eventListeners[type]);
            foreach (var listener in listeners)
            {
                listener?.Invoke(evt);
            }
            return true;
        }

        // Convenience method for creating and dispatching events
        public bool DispatchEvent(string type, object target = null)
        {
            var evt = new Event(type, target);
            return DispatchEvent(evt);
        }
    }

    /// <summary>
    /// Simple event class mimicking Flash Event.
    /// </summary>
    public class Event
    {
        public string Type { get; }
        public object Target { get; }

        public Event(string type, object target = null)
        {
            Type = type;
            Target = target;
        }

        // Common event types
        public const string ADDED_TO_STAGE = "addedToStage";
        public const string REMOVED_FROM_STAGE = "removedFromStage";
        public const string COMPLETE = "complete";
        public const string CHANGE = "change";
        public const string MOUSE_DOWN = "mouseDown";
        public const string MOUSE_UP = "mouseUp";
        public const string KEY_DOWN = "keyDown";
        public const string KEY_UP = "keyUp";
    }
}