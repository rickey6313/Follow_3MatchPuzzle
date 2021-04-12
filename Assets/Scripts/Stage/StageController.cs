using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    private bool isInit;
    private Stage stage;

    // Start is called before the first frame update
    void Start()
    {
        InitStage();
    }
    private void InitStage()
    {
        if (isInit)
            return;
        isInit = true;

        BuildStage();
    }

    private void BuildStage()
    {
        // Stage¸¦ ±¸¼º
        
    }
}
