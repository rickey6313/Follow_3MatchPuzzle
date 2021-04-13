using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageReader
{
    public static StageInfo LoadStage(int nStage)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(nStage)}");

        if(textAsset != null)
        {
            StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

            Debug.Log(stageInfo.DoValidation());

            return stageInfo;
        }

        return null;
    }

    private static string GetFileName(int nStage)
    {
        return string.Format("stage_{0:D4}", nStage);
    }
}
