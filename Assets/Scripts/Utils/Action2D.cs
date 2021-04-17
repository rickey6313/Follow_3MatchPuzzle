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

    public static IEnumerator Scale(Transform target, float toScale, float speed)
    {
        // 1. 방향 결정 : 커지는 방향이면 +, 줄어드는 방향이면 -
        bool bInc = target.localScale.x < toScale;
        float fDir = bInc ? 1 : -1;

        float factor;
        while(true)
        {
            factor = Time.deltaTime * speed * fDir;
            target.localScale = new Vector3(target.localScale.x + factor, target.localScale.y + factor, target.localScale.z);

            if ((!bInc && target.localScale.x <= toScale) || (bInc && target.localScale.x >= toScale))
                break;

            yield return null;
        }

        yield break;
    }
}
