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

    public Board(int _row, int _col)
    {
        row = _row;
        col = _col;

        cells = new Cell[row, col];
        blocks = new Block[row, col];

    }
}
