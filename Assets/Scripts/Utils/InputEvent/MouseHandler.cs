using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : IInputHandlerBase
{
    public bool isTouchDown => Input.GetButtonDown("Fire1");

    public bool isTouchUp => Input.GetButtonUp("Fire1");

    public Vector2 inputPosition => Input.mousePosition;
}
