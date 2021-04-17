using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage
{
    private int row;
    private int col;
    public int maxRow { get { return board.maxRow; } }
    public int maxCol { get { return board.maxCol; } }

    private Board board;
    public Board GetBoard { get { return board; } }

    private StageBuilder stageBuilder;

    public Block[,] blocks { get { return board.GetBlocks; } }
    public Cell[,] cells { get { return board.GetCells; } }

    public Stage(StageBuilder _stageBuilder, int _row, int _col)
    {
        stageBuilder = _stageBuilder;
        board = new Board(_row, _col);
    }

    internal void ComposeStage(GameObject _cellPrefab, GameObject _blockPrefab, Transform _container)
    {
        board.ComposeStage(stageBuilder, _cellPrefab, _blockPrefab, _container);
    }

    public bool IsOnValideBlock(Vector2 point, out BlockPos blockPos)
    {
        // 1. Local 좌표 -> 보드의 블럭 인덱스로 변환
        Vector2 pos = new Vector2(point.x + (maxCol / 2.0f), point.y + (maxRow / 2.0f));
        int nRow = (int)pos.y;
        int nCol = (int)pos.x;

        // 리턴할 블럭 인덱스 생성
        blockPos = new BlockPos(nRow, nCol);

        // 2. 스와이프 가능한지 체크
        return board.IsSwipeable(nRow, nCol);
        
    }

    public bool IsInsideBoard(Vector2 ptOrg)
    {
        // 계산의 편의를 위해서 (0, 0)을 기준으로 좌표를 이동한다. 
        // 8 x 8 보드인 경우: x(-4 ~ +4), y(-4 ~ +4) -> x(0 ~ +8), y(0 ~ +8) 
        Vector2 centerPoint = new Vector2(ptOrg.x + (maxCol / 2.0f), ptOrg.y + (maxRow / 2.0f));

        if (centerPoint.y < 0 || centerPoint.x < 0 || centerPoint.y > maxRow || centerPoint.x > maxCol)
        {
            return false;
        }
        return true;
    }

    public bool IsValideSwipe(int nRow, int nCol, Swipe swipeDir)
    {
        switch(swipeDir)
        {
            case Swipe.DOWN:return nRow > 0;
            case Swipe.UP: return nRow < maxRow -1;
            case Swipe.LEFT: return nCol > 0;
            case Swipe.RIGHT: return nCol < maxCol -1;
            default:
                return false;
        }
    }

    public IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir, Returnable<bool> actionResult)
    {
        actionResult.value = false;

        // 1. 스와이프 되는 상대 블럭 위치를 구한다.
        int nSwipeRow = nRow, nSwipeCol = nCol;
        nSwipeRow += swipeDir.GetTargetRow();
        nSwipeCol += swipeDir.GetTargetCol();

        Debug.Assert(nRow != nSwipeRow || nCol != nSwipeCol, "Invalid Swipe : ({nSwipeRow}, {nSwipeCol})");
        Debug.Assert(nSwipeRow >= 0 && nSwipeRow < maxRow && nSwipeCol >= 0 && nSwipeCol < maxCol, $"Swipe 타겟 블럭 인덱스 오류 = ({nSwipeRow}, {nSwipeCol}) ");

        // 2. 스와이프 가능한 블럭인지 체크한다
        if(board.IsSwipeable(nSwipeRow, nSwipeCol))
        {
            // 2.1 스와이프 대상 블럭(소스, 타겟)과 각 블럭의 이동전 위치를 저장한다.
            Block targetBlock = blocks[nSwipeRow, nSwipeCol];
            Block baseBlock = blocks[nRow, nCol];

            Vector3 basePos = baseBlock.blockObj.transform.position;
            Vector3 targetPos = targetBlock.blockObj.transform.position;

            // 2.2 스와이프 액션을 실행한다.
            if(targetBlock.IsSwipeable(baseBlock))
            {
                // 2.2.1 상대방의 블럭 위치로 이동하는 애니메이션을 수행한다.
                baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);

                yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                // 2.2.2 Board에 저장된 블럭의 위치를 교환한다.
                blocks[nRow, nCol] = targetBlock;
                blocks[nSwipeRow, nSwipeCol] = baseBlock;

                actionResult.value = true;
            }
        }
        yield break;
    }

    public IEnumerator Evaluate(Returnable<bool> matchResult)
    {
        yield return board.Evaluate(matchResult);
    }
}
