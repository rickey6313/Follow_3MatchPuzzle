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
        int nRow = (int)pos.x;
        int nCol = (int)pos.y;

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
}
