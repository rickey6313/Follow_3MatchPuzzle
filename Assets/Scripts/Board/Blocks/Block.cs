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

        // cell 생성
        GameObject newObj = Object.Instantiate(_blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // 컨테이너(Board)의 차일드로 cell을 포함
        newObj.transform.parent = _containerObj;
        // Cell 오브젝트에 적용된 CellBehaviour 가져와서 보관 및 SetCell
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
}
