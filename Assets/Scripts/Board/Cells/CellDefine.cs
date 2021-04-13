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

static class CellTypeMethod
{
    /// <summary>
    /// ����� ��ġ�� �� �ִ� Ÿ������ üũ�Ѵ�. ���� ��ġ�� ���� ���¿� �������
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static bool IsBlockAllocatableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }

    /// <summary>
    /// ���� �ٸ� ��ġ�� �̵� ������ Ÿ������ üũ�Ѵ�. ���� �����ϰ� �ִ� ���¿� �������
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static bool IsBlockMovableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }
}
