using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    protected CellType cellType;
    public CellType GetCellType
    {
        get { return cellType; }
        set { cellType = value; }
    }

    public Cell(CellType type)
    {
        cellType = type;
    }
}
