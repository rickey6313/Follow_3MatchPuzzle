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


    // 입력 상태 처리 플래스, 유효한 블럭을 클릭한 경우 true
    private bool bTouchDown;
    // 블럭 위치(보드에 저장된 위치)
    private BlockPos mBlockDownPos;
    // Down 위치(보드 기준 Local 좌표)
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
        // Stage를 구성
        stage = StageBuilder.BuildStage(nStage);
        stage.ComposeStage(cellPrefab, blockPrefab, container);
        mActionManager = new ActionManager(container, stage);
    }

    private void OnInputHandler()
    {        
        if(!bTouchDown && mInputManager.isTouchDown)
        {
            // 1.1 보드 기준 Local 좌표를 구한다.
            Vector2 point = mInputManager.touch2BoardPosition;

            // 1.2 Play 영역(보드)에서 클릭하지 않는 경우는 무시
            if (!stage.IsInsideBoard(point))
                return;

            // 1.3 클릭한 위치의 블럭을 구한다.
            BlockPos blockPos;
            if(stage.IsOnValideBlock(point, out blockPos))
            {
                // 1.3.1  유효한(스와이프 가능한) 블럭에서 클릭한 경우
                bTouchDown = true;          // 클릭 상태 플래그 ON
                mBlockDownPos = blockPos;   // 클릭한 블럭의 위치 저장
                mClickPos = point;          // 클릭한 Local 좌표 저장
            }
        }
        else if(bTouchDown && mInputManager.isTouchUp)
        {
            // 2.1 보드 기준 Local 좌표를 구한다
            Vector2 point = mInputManager.touch2BoardPosition;
            // 2.2 스와이프 방향을 구한다.
            Swipe swipeDir = mInputManager.EvalSwipeDir(mClickPos, point);

            if (swipeDir != Swipe.NA)
                mActionManager.DoSwipeAction(mBlockDownPos.row, mBlockDownPos.col, swipeDir);

            bTouchDown = false; // 클릭 상태 플래그 OFF
        }
    }
}
