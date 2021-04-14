using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    private BlockType blockType;
    public BlockType GetBlockType
    {
        get{ return blockType; }
        set { blockType = value; }
    }

    // �� �ߺ� ����, Shuffle�ÿ� �ߺ��˻翡 ���
    private Vector2Int vtDuplicate;

    // ���ι��� �ߺ� �˻� �� ���
    public int horzDuplicate
    {
        get { return vtDuplicate.x; }
        set { vtDuplicate.x = value; }
    }

    // ���ι��� �ߺ� �˻� �� ���
    public int vertDuplicate
    {
        get { return vtDuplicate.y; }
        set { vtDuplicate.y = value; }
    }

   // Block�� ����� GameObject�� Transform�� ���Ѵ�.
    public Transform blockObj { get { return blockBehaviour?.transform; } }

    protected BlockBehaviour blockBehaviour;
    public BlockBehaviour Behaviour
    {
        get { return blockBehaviour; }
        set
        {
            blockBehaviour = value;
            blockBehaviour.SetBlock(this);
        }
    }

    protected BlockBreed mBreed;
    public BlockBreed breed
    {
        get { return mBreed; }
        set
        {
            mBreed = value;
            blockBehaviour?.UpdateView(true);
        }
    }

    public Block(BlockType type)
    {
        blockType = type;
    }

    internal Block InstantiateBlockObj(GameObject _blockPrefab, Transform _containerObj)
    {
        if (isValidate() == false)
        {
            Debug.Log("isValidate null");
            return null;
        }

        // cell ����
        GameObject newObj = Object.Instantiate(_blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // �����̳�(Board)�� ���ϵ�� cell�� ����
        newObj.transform.parent = _containerObj;
        // Cell ������Ʈ�� ����� CellBehaviour �����ͼ� ���� �� SetCell
        Behaviour = newObj.transform.GetComponent<BlockBehaviour>();

        return this;
    }

    public void Move(float x, float y)
    {
        blockBehaviour.transform.position = new Vector3(x, y);
    }

    public bool isValidate()
    {
        return GetBlockType != BlockType.EMPTY;
    }

    public void ResetDuplicationInfo()
    {
        vtDuplicate.x = 0;
        vtDuplicate.y = 0;
    }

    public bool IsEqual(Block block)
    {
        if(breed == block.breed)
        {
            return true;
        }
        return false;
    }
    
    public bool IsMatchableBlock()
    {
        return !(GetBlockType == BlockType.EMPTY);
    }
}
