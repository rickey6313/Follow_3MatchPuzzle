using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    // �����, ���� ��ġ�� �� ����, ��� ���
    EMPTY = 0,
    // ����ִ� �⺻ ��
    BASIC = 1,
    // ������ ��ֹ�, ��ȭ����
    FIXTURE= 2,
    // ���� : �� �̵� ����, Clear�Ǹ� BASIC, ��� : CellBg
    JELLY = 3
}

