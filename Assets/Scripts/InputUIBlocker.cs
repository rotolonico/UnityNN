using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class InputUIBlocker : MonoBehaviour
{
    public static bool BlockedByUI;
    private static EventTrigger currentTrigger;

    private EventTrigger eventTrigger;

    private void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null) return;

        var enterUIEntry = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};

        enterUIEntry.callback.AddListener(eventData => { EnterUI(eventTrigger); });
        eventTrigger.triggers.Add(enterUIEntry);

        var exitUIEntry = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
        exitUIEntry.callback.AddListener(eventData => { ExitUI(eventTrigger); });
        eventTrigger.triggers.Add(exitUIEntry);
    }

    private static void EnterUI(EventTrigger trigger)
    {
        BlockedByUI = true;
        currentTrigger = trigger;
    }

    private static void ExitUI(EventTrigger trigger)
    {
        if (trigger == currentTrigger) BlockedByUI = false;
    }
}