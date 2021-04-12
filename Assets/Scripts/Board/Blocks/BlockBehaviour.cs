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
}
