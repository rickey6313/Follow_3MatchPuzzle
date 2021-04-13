using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellFactory
{
    public static Cell SpawnCell(StageInfo stageInfo, int nRow, int nCol)
    {
        Cell cell;

        
        switch (stageInfo.GetCellType(nRow, nCol))
        {
            default:

            case CellType.EMPTY:
                return cell = new Cell(CellType.EMPTY);
            case CellType.BASIC:
                return cell = new Cell(CellType.BASIC);
            case CellType.FIXTURE:
                return cell = new Cell(CellType.FIXTURE);
            case CellType.JELLY:
                return cell = new Cell(CellType.JELLY);
        }
    }
}
