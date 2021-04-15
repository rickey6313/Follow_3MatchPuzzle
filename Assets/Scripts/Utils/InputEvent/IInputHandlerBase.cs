using UnityEngine;

public interface IInputHandlerBase
{
    public bool isTouchDown { get; }
    public bool isTouchUp { get; }
    public Vector2 inputPosition { get; }
}
