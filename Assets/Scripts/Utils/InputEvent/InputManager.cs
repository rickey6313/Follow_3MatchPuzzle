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
        // 1. ��ũ�� ��ǥ -> ���� ��ǥ
        Vector3 vtMousePos = Camera.main.ScreenToWorldPoint(vtInput);

        // 2. �����̳� local ��ǥ��� ��ȯ(�����̳� ��ġ �̵��ÿ��� �����̳� ������ ���� ��ǥ���̹Ƿ� ȭ�� ������ ����)
        Vector3 vtContainerLocal = mContainer.transform.InverseTransformPoint(vtMousePos);

        return vtContainerLocal;
    }

    public Swipe EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
    {
        return TouchEvaluator.EvalSwipeDir(vtStart, vtEnd);
    }
}
