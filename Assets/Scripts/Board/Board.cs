using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private int row;
    private int col;

    public int maxRow { get { return row; } }
    public int maxCol { get { return col; } }

    private Cell[,] cells;
    public Cell[,] GetCells { get { return cells; } }

    private Block[,] blocks;
    public Block[,] GetBlocks { get { return blocks; } }

    private StageBuilder stageBuilder;
    private Transform container;
    private GameObject cellPrefab;
    private GameObject blockPrefab;

    public Board(int _row, int _col)
    {
        row = _row;
        col = _col;

        cells = new Cell[row, col];
        blocks = new Block[row, col];
    }

    internal void ComposeStage(StageBuilder _stageBuilder, GameObject _cellPrefab, GameObject _blockPrefab, Transform _container)
    {
        stageBuilder = _stageBuilder;
        container = _container;
        cellPrefab = _cellPrefab;
        blockPrefab = _blockPrefab;

        // Shuffler »ý¼º ¹× ¼ÅÇÃ
        BoardShuffler shuffler = new BoardShuffler(this, true);
        shuffler.Shuffle();

        float initX = CalcInitX(0.5f);
        float initY = CalcInitX(0.5f);

        for (int nRow = 0; nRow < row; nRow++)
        {
            for (int nCol = 0; nCol < col; nCol++)
            {
                Cell cell = cells[nRow, nCol].InstantiateCellObj(_cellPrefab, _container);
                cell.Move(initX + nCol, initY + nRow);

                Block block = blocks[nRow, nCol].InstantiateBlockObj(_blockPrefab, _container);
                if (block == null) continue;
                block.Move(initX + nCol, initY + nRow);
            }
        }
    }

    public bool CanShuffle(int nRow, int nCol, bool loading)
    {
        if (!cells[nRow, nCol].GetCellType.IsBlockMovableType())
            return false;

        return true;
    }

    public void ChangeBlock(Block block, BlockBreed notAllowedBreed)
    {
        BlockBreed genBreed;

        while(true)
        {
            genBreed = (BlockBreed)UnityEngine.Random.Range(0, 6);

            if (notAllowedBreed == genBreed)
                continue;

            break;
        }

        block.breed = genBreed;
    }

    public float CalcInitX(float offset = 0)
    {
        return -col / 2.0f + offset;
    }

    public float CalcInitY(float offset = 0)
    {
        return -row / 2.0f + offset;
    }
}
