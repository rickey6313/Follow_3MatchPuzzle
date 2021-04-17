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
        // 1. Local ��ǥ -> ������ �� �ε����� ��ȯ
        Vector2 pos = new Vector2(point.x + (maxCol / 2.0f), point.y + (maxRow / 2.0f));
        int nRow = (int)pos.y;
        int nCol = (int)pos.x;

        // ������ �� �ε��� ����
        blockPos = new BlockPos(nRow, nCol);

        // 2. �������� �������� üũ
        return board.IsSwipeable(nRow, nCol);
        
    }

    public bool IsInsideBoard(Vector2 ptOrg)
    {
        // ����� ���Ǹ� ���ؼ� (0, 0)�� �������� ��ǥ�� �̵��Ѵ�. 
        // 8 x 8 ������ ���: x(-4 ~ +4), y(-4 ~ +4) -> x(0 ~ +8), y(0 ~ +8) 
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

        // 1. �������� �Ǵ� ��� �� ��ġ�� ���Ѵ�.
        int nSwipeRow = nRow, nSwipeCol = nCol;
        nSwipeRow += swipeDir.GetTargetRow();
        nSwipeCol += swipeDir.GetTargetCol();

        Debug.Assert(nRow != nSwipeRow || nCol != nSwipeCol, "Invalid Swipe : ({nSwipeRow}, {nSwipeCol})");
        Debug.Assert(nSwipeRow >= 0 && nSwipeRow < maxRow && nSwipeCol >= 0 && nSwipeCol < maxCol, $"Swipe Ÿ�� �� �ε��� ���� = ({nSwipeRow}, {nSwipeCol}) ");

        // 2. �������� ������ ������ üũ�Ѵ�
        if(board.IsSwipeable(nSwipeRow, nSwipeCol))
        {
            // 2.1 �������� ��� ��(�ҽ�, Ÿ��)�� �� ���� �̵��� ��ġ�� �����Ѵ�.
            Block targetBlock = blocks[nSwipeRow, nSwipeCol];
            Block baseBlock = blocks[nRow, nCol];

            Vector3 basePos = baseBlock.blockObj.transform.position;
            Vector3 targetPos = targetBlock.blockObj.transform.position;

            // 2.2 �������� �׼��� �����Ѵ�.
            if(targetBlock.IsSwipeable(baseBlock))
            {
                // 2.2.1 ������ �� ��ġ�� �̵��ϴ� �ִϸ��̼��� �����Ѵ�.
                baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);

                yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                // 2.2.2 Board�� ����� ���� ��ġ�� ��ȯ�Ѵ�.
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
