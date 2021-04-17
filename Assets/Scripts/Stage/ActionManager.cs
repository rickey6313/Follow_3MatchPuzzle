using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    // Board
    private Transform mContainer;    
    private Stage mStage;
    // �ڷ�ƾ ȣ�� �� �ʿ��� MonoBeaviour
    private MonoBehaviour mMonoBehaviour;
    // �׼� ���� ���� : �������� ��� true
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

    //  Swipe �׼� ����
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

            // 1. Swipe Action ����
            Returnable<bool> bSwipedBlock = new Returnable<bool>(false);
            yield return mStage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipedBlock);

            // 2. Swipe ������ ��� ���带 ��(��ġ�� ����, ��� ���, ���� Spawn ��) �Ѵ�.
            if(bSwipedBlock.value)
            {
                Returnable<bool> bMathBlock = new Returnable<bool>(false);
                yield return EvaluateBoard(bMathBlock);

                // Swipe�� ���� ��ġ���� ���� ��쿡 ������ ����
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
