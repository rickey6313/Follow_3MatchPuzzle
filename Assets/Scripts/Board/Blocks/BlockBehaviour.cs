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

    // ��Ī�� GameObject ����
    public void DoActionClear()
    {        
        StartCoroutine(CoStartSimpleExplosion(true));
    }

    // ���� ������ ��, GameObject�� ����
    private IEnumerator CoStartSimpleExplosion(bool bDestroy = true)
    {
        yield return Action2D.Scale(transform, Constants.BLOCK_DESTROY_SCALE, 4f);

        // 1. ���Ľ�Ű�� ȿ�� ���� : �� ��ü�� Clear ȿ���� �����Ѵ� (��� �� ����)
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
