using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHandler : IInputHandlerBase
{
    public bool isTouchDown => Input.GetTouch(0).phase == TouchPhase.Began;

    public bool isTouchUp => Input.GetTouch(0).phase == TouchPhase.Ended;

    public Vector2 inputPosition => Input.GetTouch(0).position;
}
