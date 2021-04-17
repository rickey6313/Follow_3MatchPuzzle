using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockActionBehaviour : MonoBehaviour
{
    public bool isMoving { get; set; }
    Queue<Vector3> mMovementQueue = new Queue<Vector3>();

    public void MoveDrop(Vector2 vtDropDistance)
    {
        mMovementQueue.Enqueue(new Vector3(vtDropDistance.x, vtDropDistance.y, 1));

        if(!isMoving)
        {
            StartCoroutine(DoActionMoveDrop());
        }
    }

    private IEnumerator DoActionMoveDrop(float acc = 1.0f)
    {
        isMoving = true;

        while(mMovementQueue.Count > 0)
        {
            Vector2 vtDestination = mMovementQueue.Dequeue();

            float duration = Constants.DROP_TIME;
            yield return CoStartDropSmooth(vtDestination, duration * acc);
        }

        isMoving = false;
        yield break;
    }

    private IEnumerator CoStartDropSmooth(Vector2 vtDestination, float duration)
    {
        //Vector3 ownPos = transform.position;
        Vector3 to = new Vector3(transform.position.x + vtDestination.x, transform.position.y - vtDestination.y, transform.position.z);
        yield return Action2D.MoveTo(transform, to, duration);
    }
}

