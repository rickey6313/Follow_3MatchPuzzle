using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    // 빈공간, 블럭이 위치할 수 없음, 드롭 통과
    EMPTY = 0,
    // 배경있는 기본 형
    BASIC = 1,
    // 고정된 장애물, 변화없음
    FIXTURE= 2,
    // 젤리 : 블럭 이동 가능, Clear되면 BASIC, 출력 : CellBg
    JELLY = 3
}

static class CellTypeMethod
{
    /// <summary>
    /// 블록이 위치할 수 있는 타입인지 체크한다. 현재 위치한 블럭의 상태와 관계없음
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static bool IsBlockAllocatableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }

    /// <summary>
    /// 블럭이 다른 위치로 이동 가능한 타입인지 체크한다. 현재 포함하고 있는 상태와 관계없음
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static bool IsBlockMovableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }
}
