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

    // 블럭 중복 개수, Shuffle시에 중복검사에 사용
    private Vector2Int vtDuplicate;

    // 가로방향 중복 검사 시 사용
    public int horzDuplicate
    {
        get { return vtDuplicate.x; }
        set { vtDuplicate.x = value; }
    }

    // 세로방향 중복 검사 시 사용
    public int vertDuplicate
    {
        get { return vtDuplicate.y; }
        set { vtDuplicate.y = value; }
    }

   // Block에 연결된 GameObject의 Transform을 구한다.
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

    // 내구도, 0이 되면 제거
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

        // cell 생성
        GameObject newObj = Object.Instantiate(_blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // 컨테이너(Board)의 차일드로 cell을 포함
        newObj.transform.parent = _containerObj;
        // Cell 오브젝트에 적용된 CellBehaviour 가져와서 보관 및 SetCell
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

    // Board의 Evaluation으로 업데이트된 블럭 자신의 매칭 상태를 평가해서 현재 상태에 부여된 동작을 수행한다.
    public bool DoEvaluation(BoardEnumerator boardEnumerator, int nRow, int nCol)
    {
        Debug.Assert(boardEnumerator != null, $"({nRow}, {nCol})");

        if (IsEvaluatable())
            return false;

        // 1. 매치 상태(클리어 조건 충족)인 경우
        if(status == BlockStatus.MATCH)
        {   
            if(questType == BlockQuestType.CLEAR_SIMPLE || boardEnumerator.IsCageTypeCell(nRow, nCol))
            {
                Debug.Assert(m_nDurability > 0, $"durability is zero : {m_nDurability}");

                durability--;
            }
            else // 특수 블럭인 경우 true 리턴
            {
                return true;
            }

            if(m_nDurability == 0)
            {
                status = BlockStatus.CLEAR;
                return false;
            }
        }

        // 2. 클리어 조건에 아직 도달하지 않은 경우 NORMAL 상태로 복귀
        status = BlockStatus.NORMAL;
        match = MatchType.NONE;
        matchCount = 0;

        return false;
    }

    // Board에 상태 조회함수 추가
    // Evaluation 대상이 되는 블럭인지 조회하는 함수
    public bool IsEvaluatable()
    {
        // 이미 처리완료(CLEAR) 되었거나, 현재 처리중인 블럭인 경우
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
