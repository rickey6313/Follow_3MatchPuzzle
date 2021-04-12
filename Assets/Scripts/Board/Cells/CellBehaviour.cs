using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
    private Cell cell;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateView(false);
    }

    public void SetCell(Cell _cell)
    {
        cell = _cell;
    }

    public void UpdateView(bool valueChanged)
    {
        if(cell.GetCellType == CellType.EMPTY)
        {
            spriteRenderer.sprite = null;
        }
    }
}
