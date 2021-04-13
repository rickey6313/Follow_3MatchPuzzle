using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public int nStage;
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
        stage = StageBuilder.BuildStage(nStage);
        stage.ComposeStage(cellPrefab, blockPrefab, container);
    }
}
