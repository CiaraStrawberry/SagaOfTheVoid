using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController {

    /// <summary>
    /// Event data passed with every event
    /// </summary>
    public struct EventData {

        /// <summary>
        /// Object triggering the event
        /// </summary>
        public GameObject TriggeringObject;

        /// <summary>
        /// Type of event triggered
        /// </summary>
        public EventTypes EventType;

        /// <summary>
        /// Data associated with event
        /// </summary>
        public object[] EventParameters;

        /// <summary>
        /// Constructor for event
        /// </summary>
        /// <param name="TriggeringObject"></param>
        /// <param name="EventType"></param>
        /// <param name="EventParameters"></param>
        internal EventData(GameObject triggeringObject, EventTypes eventType, params object[] eventParameters) {
            TriggeringObject = triggeringObject;
            EventType = eventType;
            EventParameters = eventParameters;
        }

    }

    /// <summary>
    /// Events in SOV
    /// </summary>
    public enum EventTypes {

        ObjectSpawned,
        ObjectWarpedIn,
        ObjectDestroyed,
        AllWidgetsHidden,
        AllGesturesDisabled,
        GestureEnabled,
        AllGesturesEnabled,
        WidgetDisplayed,
        ObjectInProximity,
        ObjectSelected,
        SoundPlayed,
        LookAtAnimated,
        MapChanged

    }

    /// <summary>
    /// Object for thread safety
    /// </summary>
    object _LockObject = new object();

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<EventTypes, List<System.Action<EventData>>> _Listeners;

    #region Singleton

    /// <summary>
    /// Controller instance
    /// </summary>
    private static EventController _controller;

    /// <summary>
    /// Hide the constructor
    /// </summary>
    private EventController() {
        // Should be thread safe, as this is only called once in a lock condition
        _Listeners = new Dictionary<EventTypes, List<System.Action<EventData>>>();
    }

    public EventController GetInstance {

        get {
            
            lock(_LockObject) {
                if (_controller == null) {
                    _controller = new EventController();
                }
            }

            return _controller;

        }

    }

    #endregion

    /// <summary>
    /// Adds a listener
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callback"></param>
    public void AttachListener(EventTypes eventType, System.Action<EventData> callback) {

        // Lock for thread safety
        lock(_LockObject) {

            // Check if the listener cache has a key set up or not
            if (!_Listeners.ContainsKey(eventType)) {
                _Listeners.Add(eventType, new List<System.Action<EventData>>());
            }

            // Add the listener
            _Listeners[eventType].Add(callback);

        }

    }

    /// <summary>
    /// Removes a listener
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callback"></param>
    public void DetatchListender(EventTypes eventType, System.Action<EventData> callback) {

        // Lock for thread safety
        lock (_LockObject) {

            // Check if the listener cache has a key set up or not
            if (_Listeners.ContainsKey(eventType)) {

                // Remove the listener
                _Listeners[eventType].Remove(callback);

            }

        }

    }

    /// <summary>
    /// Triggers an event, calls all listeners
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public void TriggerEvent(EventTypes eventType, EventData data) {

        lock(_LockObject) {

            if (_Listeners.ContainsKey(eventType)) {

                foreach (System.Action<EventData> cb in _Listeners[eventType]) {

                    cb.DynamicInvoke(data);

                }

            }

        }

    }

}
