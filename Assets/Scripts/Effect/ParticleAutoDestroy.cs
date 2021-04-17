using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(CoCheckAlive());
    }

    private IEnumerator CoCheckAlive()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!GetComponent<ParticleSystem>().IsAlive(true))
            {
                Destroy(this.gameObject);
                break;
            }
        }
    }
}
