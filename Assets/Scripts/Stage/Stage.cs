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

    //public void PrintAll()
    //{
    //    System.Text.StringBuilder strCells = new System.Text.StringBuilder();
    //    System.Text.StringBuilder strBlocks = new System.Text.StringBuilder();

    //    for(int row = maxRow -1; row >=0; row--)
    //    {
    //        for(int col = 0; col < maxCol; col++)
    //        {
    //            strCells.Append($"{cells[row, col].GetCellType},");
    //            strBlocks.Append($"{blocks[row, col].GetBlockType},");
    //        }
    //        strCells.Append("\n");
    //        strBlocks.Append("\n");
    //    }

    //    Debug.Log(strCells.ToString());
    //    Debug.Log(strBlocks.ToString());
    //}
}
