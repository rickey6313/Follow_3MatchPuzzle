using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    [SerializeField]
    private Camera targetTransform;
    [SerializeField]
    private float boardUnit;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("camera aspect : " + targetTransform.aspect);
        targetTransform.orthographicSize = boardUnit / targetTransform.aspect;
    }
}
