using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ڷ�ƾ ����� �����ϱ� ���� ���� Ŭ����
/// </summary>
/// <typeparam name="T"></typeparam>
public class Returnable<T>
{
    public T value { get; set; }

    public Returnable(T value)
    {
        this.value = value;
    }
}
