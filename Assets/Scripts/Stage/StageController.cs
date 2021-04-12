using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    private bool isInit;
    private Stage stage;

    [SerializeField]
    private Transform container;
    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private GameObject blockPrefab;


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

        //stage.PrintAll();
    }

    private void BuildStage()
    {
        // Stage¸¦ ±¸¼º
        stage = StageBuilder.BuildStage(0, 9, 9);
        stage.ComposeStage(cellPrefab, blockPrefab, container);
    }
}
