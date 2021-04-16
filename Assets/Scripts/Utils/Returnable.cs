using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 코루틴 결과를 수신하기 위한 범용 클래스
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
