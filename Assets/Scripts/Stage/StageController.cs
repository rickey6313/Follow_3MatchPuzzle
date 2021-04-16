using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public int nStage;
    private bool isInit;
    private Stage stage;

    [SerializeField]
    private Transform container;
    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private GameObject blockPrefab;

    private InputManager mInputManager;
    private ActionManager mActionManager;


    // �Է� ���� ó�� �÷���, ��ȿ�� ���� Ŭ���� ��� true
    private bool bTouchDown;
    // �� ��ġ(���忡 ����� ��ġ)
    private BlockPos mBlockDownPos;
    // Down ��ġ(���� ���� Local ��ǥ)
    private Vector3 mClickPos;

    // Start is called before the first frame update
    void Start()
    {
        InitStage();
    }
    private void InitStage()
    {
        if (isInit)
            return;
        isInit = true;
        mInputManager = new InputManager(container);
        BuildStage();
    }

    private void Update()
    {
        if (!isInit)
            return;

        OnInputHandler();
    }

    private void BuildStage()
    {
        // Stage�� ����
        stage = StageBuilder.BuildStage(nStage);
        stage.ComposeStage(cellPrefab, blockPrefab, container);
        mActionManager = new ActionManager(container, stage);
    }

    private void OnInputHandler()
    {        
        if(!bTouchDown && mInputManager.isTouchDown)
        {
            // 1.1 ���� ���� Local ��ǥ�� ���Ѵ�.
            Vector2 point = mInputManager.touch2BoardPosition;

            // 1.2 Play ����(����)���� Ŭ������ �ʴ� ���� ����
            if (!stage.IsInsideBoard(point))
                return;

            // 1.3 Ŭ���� ��ġ�� ���� ���Ѵ�.
            BlockPos blockPos;
            if(stage.IsOnValideBlock(point, out blockPos))
            {
                // 1.3.1  ��ȿ��(�������� ������) ������ Ŭ���� ���
                bTouchDown = true;          // Ŭ�� ���� �÷��� ON
                mBlockDownPos = blockPos;   // Ŭ���� ���� ��ġ ����
                mClickPos = point;          // Ŭ���� Local ��ǥ ����
            }
        }
        else if(bTouchDown && mInputManager.isTouchUp)
        {
            // 2.1 ���� ���� Local ��ǥ�� ���Ѵ�
            Vector2 point = mInputManager.touch2BoardPosition;
            // 2.2 �������� ������ ���Ѵ�.
            Swipe swipeDir = mInputManager.EvalSwipeDir(mClickPos, point);

            if (swipeDir != Swipe.NA)
                mActionManager.DoSwipeAction(mBlockDownPos.row, mBlockDownPos.col, swipeDir);

            bTouchDown = false; // Ŭ�� ���� �÷��� OFF
        }
    }
}
