using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public enum EEvent
{
    GameReady,
    GameStart,
    GamePause,
    GameResume,

    TimerTick,
    MonsterHitCharacter,
    ChangeHP,

    GameOver,
}



public class EventManager : MonoBehaviour
{
    public delegate void EventDelegate(object param);

    private readonly Dictionary<EEvent, List<EventDelegate>> delegateListDic = new();

    internal void AddEventDelegate(EEvent eventType, EventDelegate eventDelegate)
    {
        if (!delegateListDic.TryGetValue(eventType, out var delegateList))
        {
            delegateList = new();
        }

        if (!delegateList.Contains(eventDelegate))
        {
#if UNITY_EDITOR
            Debug.Log($"EventManager[{eventType}]: Delegate {eventDelegate.Target.GetType()}:{eventDelegate.Method.Name} added on {delegateList.Count}");
#endif
            delegateList.Add(eventDelegate);
            delegateListDic[eventType] = delegateList;
        }
    }

    internal void RemoveEventDelegate(EEvent eventType, EventDelegate eventDelegate)
    {
        if (delegateListDic.TryGetValue(eventType, out var delegateList))
        {
            if (delegateList.Contains(eventDelegate))
            {
                delegateList.Remove(eventDelegate);
                if (delegateList.Count.Equals(0))
                {
                    delegateListDic.Remove(eventType);
                }
#if UNITY_EDITOR
                Debug.Log($"EventManager[{eventType}]: Delegate {eventDelegate.Target.GetType()}:{eventDelegate.Method.Name} removed then {delegateList.Count} left");
#endif
            }
        }
    }

    internal void ClearEventDelegate(EEvent eventType)
    {
        if (delegateListDic.TryGetValue(eventType, out var delegateList))
        {
            delegateList.Clear();
            delegateListDic.Remove(eventType);
        }
    }

    internal void SendEvent(EEvent eventType, object parameter = null)
    {
        if (delegateListDic.TryGetValue(eventType, out var delegateList))
        {
            foreach (var del in delegateList)
            {
                del.Invoke(parameter);
            }
        }
    }

    internal void PrintEvents(string prefix = "")
    {
        string log = $"/** EventManager ({prefix})";

        foreach (var pair in delegateListDic)
        {
            log += $"\n{pair.Key}:";
            foreach (var del in pair.Value)
            {
                log += $"\n\t{(del.Target is not null ? del.Target.GetType() : "__NULL__")}:{del.Method.Name}";
            }
        }

        log += " **/";

        Debug.Log(log);
    }
}
