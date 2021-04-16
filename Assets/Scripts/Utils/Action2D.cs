using System.Collections;
using UnityEngine;

public static class Action2D
{
    /// <summary>
    /// ������ �ð����� ������ ��ġ�� �̵��Ѵ�.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="to"></param>
    /// <param name="duration"></param>
    /// <param name="bSelfRemove"></param>
    /// <returns></returns>
    public static IEnumerator MoveTo(Transform target, Vector3 to, float duration, bool bSelfRemove = false)
    {
        Vector2 startPos = target.transform.position;

        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.transform.position = Vector2.Lerp(startPos, to, elapsed/duration);

            yield return null;
        }

        target.position = to;

        if (bSelfRemove)
            Object.Destroy(target.gameObject, 0.1f);

        yield break;
    }
}
