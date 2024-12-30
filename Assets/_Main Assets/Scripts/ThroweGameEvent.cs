using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroweGameEvent : MonoBehaviour
{
    [SerializeField] private GameEvent throweGameEvent;

    public void EventRaise()
    {
        throweGameEvent.Raise();
    }
}