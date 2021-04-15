using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    // Transform Board 
    private Transform mContainer;

#if UNITY_ANDROID && !UNITY_EDITOR
    private IInputHandlerBase mInputHandler = new TouchHandler();
#else
    private IInputHandlerBase mInputHandler = new MouseHandler();
#endif
    public InputManager(Transform container)
    {
        mContainer = container;
    }

    public bool isTouchDown => mInputHandler.isTouchDown;
    public bool isTouchUp => mInputHandler.isTouchUp;
    public Vector2 touchPosition => mInputHandler.inputPosition;
    public Vector2 touch2BoardPosition => TouchToPosition(mInputHandler.inputPosition);

    private Vector2 TouchToPosition(Vector3 vtInput)
    {
        // 1. 스크린 좌표 -> 월드 좌표
        Vector3 vtMousePos = Camera.main.ScreenToWorldPoint(vtInput);

        // 2. 컨테이너 local 좌표계로 변환(컨테이너 위치 이동시에도 컨테이너 기준의 로컬 좌표계이므로 화면 구성이 편함)
        Vector3 vtContainerLocal = mContainer.transform.InverseTransformPoint(vtMousePos);

        return vtContainerLocal;
    }

    public Swipe EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
    {
        return TouchEvaluator.EvalSwipeDir(vtStart, vtEnd);
    }
}
