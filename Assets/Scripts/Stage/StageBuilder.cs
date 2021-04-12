using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder
{
    private int nStage;

    public StageBuilder(int _stage)
    {
        nStage = _stage;
    }

    public Stage ComposeStage(int _row, int _col)
    {
        Stage stage = new Stage(this, _row, _col);

        for(int nRow = 0; nRow < _row; nRow++)
        {
            for (int nCol = 0; nCol < _row; nCol++)
            {
                stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
            }
        }
        return stage;
    }

    private Block SpawnBlockForStage(int nRow, int nCol)
    {
        return nRow == nCol ? SpawnEmptyBlock() : SpawnBlock();
    }

    public Block SpawnBlock()
    {
        return BlockFactory.SpawnBlock(BlockType.BASIC);
    }

    public Block SpawnEmptyBlock()
    {
        Block newBlock = BlockFactory.SpawnBlock(BlockType.EMPTY);

        return newBlock;
    }

    private Cell SpawnCellForStage(int nRow, int nCol)
    {
        return new Cell(nRow == nCol ? CellType.EMPTY : CellType.BASIC);
        //return new Cell(CellType.BASIC);
    }

    public static Stage BuildStage(int nStage, int row, int col)
    {
        StageBuilder _stageBuilder = new StageBuilder(nStage);
        Stage stage = _stageBuilder.ComposeStage(row, col);

        return stage;
    }

}
