using System.Collections;
using UnityEngine;

public static class Action2D
{
    /// <summary>
    /// 지정된 시간동안 지정된 위치로 이동한다.
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
