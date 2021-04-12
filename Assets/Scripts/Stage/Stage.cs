using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage
{
    private int row;
    private int col;
    public int maxRow { get { return row; } }
    public int maxCol { get { return col; } }

    private Board board;
    public Board GetBoard { get { return board; } }

    public Stage(int _row, int _col)
    {

    }
}
