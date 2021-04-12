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

