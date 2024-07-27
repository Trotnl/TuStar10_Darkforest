using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackJoystick : FixedJoystick
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        EventManager.Trigger(EEventType.joystick_attack_up.ToString(), Direction);

        base.OnPointerUp(eventData);
    }
}
