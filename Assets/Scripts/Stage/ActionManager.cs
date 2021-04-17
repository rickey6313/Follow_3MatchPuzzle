using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    // Board
    private Transform mContainer;    
    private Stage mStage;
    // 코루틴 호출 시 필요한 MonoBeaviour
    private MonoBehaviour mMonoBehaviour;
    // 액션 실행 상태 : 실행중인 경우 true
    private bool m_bRunning;

    public ActionManager(Transform container, Stage stage)
    {
        mContainer = container;
        mStage = stage;
        mMonoBehaviour = container.gameObject.GetComponent<MonoBehaviour>();
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return mMonoBehaviour.StartCoroutine(routine);
    }

    //  Swipe 액션 실행
    public void DoSwipeAction(int nRow, int nCol, Swipe swipeDir)
    {
        Debug.Assert(nRow >= 0 && nRow < mStage.maxRow && nCol >= 0 && nCol < mStage.maxCol);

        if(mStage.IsValideSwipe(nRow, nCol, swipeDir))
        {
            StartCoroutine(CoDoSwipeAction(nRow, nCol, swipeDir));
        }
    }

    private IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir)
    {
        if(!m_bRunning)
        {
            m_bRunning = true;

            // 1. Swipe Action 수행
            Returnable<bool> bSwipedBlock = new Returnable<bool>(false);
            yield return mStage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipedBlock);

            // 2. Swipe 성공한 경우 보드를 평가(매치블럭 삭제, 빈블럭 드롭, 새블럭 Spawn 등) 한다.
            if(bSwipedBlock.value)
            {
                Returnable<bool> bMathBlock = new Returnable<bool>(false);
                yield return EvaluateBoard(bMathBlock);

                // Swipe한 블럭이 매치되지 않은 경우에 원상태 복귀
                if(!bMathBlock.value)
                {
                    yield return mStage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipedBlock);
                }
            }
            m_bRunning = false;
        }
        yield break;
    }

    private IEnumerator EvaluateBoard(Returnable<bool> matchResult)
    {
        yield return mStage.Evaluate(matchResult);
    }
}
