using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder
{
    private int nStage;
    private StageInfo mStageInfo;

    public StageBuilder(int _stage)
    {
        nStage = _stage;
    }

    public Stage ComposeStage()
    {        
        mStageInfo = LoadStage(nStage);
        
        Stage stage = new Stage(this, mStageInfo.row, mStageInfo.col);

        for (int nRow = 0; nRow < mStageInfo.row; nRow++)
        {
            for (int nCol = 0; nCol < mStageInfo.col; nCol++)
            {
                stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
            }
        }
        return stage;
    }

    public StageInfo LoadStage(int nStage)
    {
        StageInfo stageInfo = StageReader.LoadStage(nStage);
        if(stageInfo != null)
        {
            Debug.Log(stageInfo.ToString());
        }
        return stageInfo;
    }

    private Block SpawnBlockForStage(int nRow, int nCol)
    {
        //return nRow == nCol ? SpawnEmptyBlock() : SpawnBlock();

        if (mStageInfo.GetCellType(nRow, nCol) == CellType.EMPTY)
            return SpawnEmptyBlock();

        return SpawnBlock();
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
        //return new Cell(nRow == nCol ? CellType.EMPTY : CellType.BASIC);

        return CellFactory.SpawnCell(mStageInfo, nRow, nCol);
    }

    public static Stage BuildStage(int nStage)
    {
        StageBuilder _stageBuilder = new StageBuilder(nStage);
        Stage stage = _stageBuilder.ComposeStage();

        return stage;
    }

}
