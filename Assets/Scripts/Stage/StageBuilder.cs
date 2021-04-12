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
        Stage stage = new Stage(_row, _col);

        for(int nRow = 0; nRow < _row; nRow++)
        {
            for (int nCol = 0; nCol < _row; nCol++)
            {

            }
        }
        return stage;
    }
}
