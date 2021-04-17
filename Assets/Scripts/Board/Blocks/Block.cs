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

    public BlockStatus status;
    public BlockQuestType questType;
    public MatchType match = MatchType.NONE;
    public short matchCount;

    // ������, 0�� �Ǹ� ����
    private int m_nDurability;
    public virtual int durability
    {
        get { return m_nDurability; }
        set { m_nDurability = value; }
    }

    private BlockActionBehaviour mBlockActionBehaviour;

    public bool isMoving
    {
        get
        {
            return blockObj != null && mBlockActionBehaviour.isMoving;
        }
    }

    public Vector2 dropDistance
    {
        set
        {
            mBlockActionBehaviour?.MoveDrop(value);
        }
    }

    public Block(BlockType type)
    {
        blockType = type;

        status = BlockStatus.NORMAL;
        questType = BlockQuestType.CLEAR_SIMPLE;
        match = MatchType.NONE;
        mBreed = BlockBreed.NA;

        m_nDurability = 1;
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
        mBlockActionBehaviour = newObj.transform.GetComponent<BlockActionBehaviour>();

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
        if(IsMatchableBlock() && breed == block.breed)
        {
            return true;
        }
        return false;
    }
    
    public bool IsMatchableBlock()
    {
        return !(GetBlockType == BlockType.EMPTY);
    }

    public void MoveTo(Vector3 to, float duration)
    {
        blockBehaviour.StartCoroutine(Action2D.MoveTo(blockObj, to, duration));
    }

    public bool IsSwipeable(Block baseBlock)
    {
        return true;
    }

    // Board�� Evaluation���� ������Ʈ�� �� �ڽ��� ��Ī ���¸� ���ؼ� ���� ���¿� �ο��� ������ �����Ѵ�.
    public bool DoEvaluation(BoardEnumerator boardEnumerator, int nRow, int nCol)
    {
        Debug.Assert(boardEnumerator != null, $"({nRow}, {nCol})");

        if (IsEvaluatable())
            return false;

        // 1. ��ġ ����(Ŭ���� ���� ����)�� ���
        if(status == BlockStatus.MATCH)
        {   
            if(questType == BlockQuestType.CLEAR_SIMPLE || boardEnumerator.IsCageTypeCell(nRow, nCol))
            {
                Debug.Assert(m_nDurability > 0, $"durability is zero : {m_nDurability}");

                durability--;
            }
            else // Ư�� ���� ��� true ����
            {
                return true;
            }

            if(m_nDurability == 0)
            {
                status = BlockStatus.CLEAR;
                return false;
            }
        }

        // 2. Ŭ���� ���ǿ� ���� �������� ���� ��� NORMAL ���·� ����
        status = BlockStatus.NORMAL;
        match = MatchType.NONE;
        matchCount = 0;

        return false;
    }

    // Board�� ���� ��ȸ�Լ� �߰�
    // Evaluation ����� �Ǵ� ������ ��ȸ�ϴ� �Լ�
    public bool IsEvaluatable()
    {
        // �̹� ó���Ϸ�(CLEAR) �Ǿ��ų�, ���� ó������ ���� ���
        if (status == BlockStatus.CLEAR || IsMatchableBlock())
            return false;

        return true;
    }

    public virtual void Destroy()
    {
        Debug.Assert(blockObj != null, $"{match}");
        blockBehaviour.DoActionClear();
    }

    public void UpdateBlockStatusMatched(MatchType matchType, bool bAccumulate = true)
    {
        status = BlockStatus.MATCH;

        if(match == MatchType.NONE)
        {
            match = matchType;
        }
        else
        {
            match = bAccumulate ? match.Add(matchType) : matchType;
        }

        matchCount = (short)matchType;
    }
}
