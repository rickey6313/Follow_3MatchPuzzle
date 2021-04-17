using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    private Block block;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private BlockConfig blockConfig;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateView(false);
    }

    public void SetBlock(Block _block)
    {
        block = _block;
    }

    public void UpdateView(bool valueChanged)
    {
        if (block.GetBlockType == BlockType.EMPTY)
        {
            spriteRenderer.sprite = null;
        }
        else if(block.GetBlockType == BlockType.BASIC)
        {
            spriteRenderer.sprite = blockConfig.basicBlockSprites[(int)block.breed];
        }
    }

    // 매칭된 GameObject 제거
    public void DoActionClear()
    {        
        StartCoroutine(CoStartSimpleExplosion(true));
    }

    // 블럭이 폭발한 후, GameObject를 삭제
    private IEnumerator CoStartSimpleExplosion(bool bDestroy = true)
    {
        yield return Action2D.Scale(transform, Constants.BLOCK_DESTROY_SCALE, 4f);

        // 1. 폭파시키는 효과 연출 : 블럭 자체의 Clear 효과를 연출한다 (모든 블럭 동일)
        GameObject explosionObj = blockConfig.GetExplosionObject(BlockQuestType.CLEAR_SIMPLE);
        ParticleSystem.MainModule newModule = explosionObj.GetComponent<ParticleSystem>().main;
        newModule.startColor = blockConfig.GetBlockColor(block.breed);

        explosionObj.SetActive(true);
        explosionObj.transform.position = this.transform.position;

        yield return new WaitForSeconds(0.1f);

        if (bDestroy)
        {
            Debug.Log("Destroy Block");
            Destroy(gameObject);
        }
        else
        {
            Debug.Assert(false, "Unkown Action : GameObject No Desttroy After Particle");
        }
    }
}
