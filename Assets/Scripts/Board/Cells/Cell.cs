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

    protected CellBehaviour cellBehaviour;
    public CellBehaviour Behaviour
    {
        get { return cellBehaviour; }
        set
        {
            cellBehaviour = value;
            cellBehaviour.SetCell(this);
        }
    }

    public Cell(CellType type)
    {
        cellType = type;
    }

    public Cell InstantiateCellObj(GameObject cellPrefab, Transform containerObj)
    {
        // cell 생성
        GameObject newobj = Object.Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // 컨테이너(Board)의 차일드로 cell을 포함
        newobj.transform.parent = containerObj;

        // Cell 오브젝트에 적용된 CellBehaviour 가져와서 보관 및 SetCell
        Behaviour = newobj.transform.GetComponent<CellBehaviour>();
        
        return this;
    }

    public void Move(float x, float y)
    {
        cellBehaviour.transform.position = new Vector3(x, y);
    }
}
