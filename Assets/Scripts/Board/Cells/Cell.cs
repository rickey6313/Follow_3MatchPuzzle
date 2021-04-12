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
        // cell ����
        GameObject newobj = Object.Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // �����̳�(Board)�� ���ϵ�� cell�� ����
        newobj.transform.parent = containerObj;

        // Cell ������Ʈ�� ����� CellBehaviour �����ͼ� ���� �� SetCell
        Behaviour = newobj.transform.GetComponent<CellBehaviour>();
        
        return this;
    }

    public void Move(float x, float y)
    {
        cellBehaviour.transform.position = new Vector3(x, y);
    }
}
