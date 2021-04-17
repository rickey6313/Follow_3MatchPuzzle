using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;

public class Board
{
    private int row;
    private int col;

    public int maxRow { get { return row; } }
    public int maxCol { get { return col; } }

    private Cell[,] cells;
    public Cell[,] GetCells { get { return cells; } }

    private Block[,] blocks;
    public Block[,] GetBlocks { get { return blocks; } }

    private StageBuilder stageBuilder;
    private Transform container;
    private GameObject cellPrefab;
    private GameObject blockPrefab;
    private BoardEnumerator mBoardEnumerator;

    public Board(int _row, int _col)
    {
        row = _row;
        col = _col;

        cells = new Cell[row, col];
        blocks = new Block[row, col];
        mBoardEnumerator = new BoardEnumerator(this);
    }

    internal void ComposeStage(StageBuilder _stageBuilder, GameObject _cellPrefab, GameObject _blockPrefab, Transform _container)
    {
        stageBuilder = _stageBuilder;
        container = _container;
        cellPrefab = _cellPrefab;
        blockPrefab = _blockPrefab;

        // Shuffler 생성 및 셔플
        BoardShuffler shuffler = new BoardShuffler(this, true);
        shuffler.Shuffle();

        float initX = CalcInitX(0.5f);
        float initY = CalcInitX(0.5f);

        for (int nRow = 0; nRow < row; nRow++)
        {
            for (int nCol = 0; nCol < col; nCol++)
            {
                Cell cell = cells[nRow, nCol].InstantiateCellObj(_cellPrefab, _container);
                cell.Move(initX + nCol, initY + nRow);

                Block block = blocks[nRow, nCol].InstantiateBlockObj(_blockPrefab, _container);
                if (block == null) continue;
                block.Move(initX + nCol, initY + nRow);
            }
        }
    }

    public bool CanShuffle(int nRow, int nCol, bool loading)
    {
        if (!cells[nRow, nCol].GetCellType.IsBlockMovableType())
            return false;

        return true;
    }

    public void ChangeBlock(Block block, BlockBreed notAllowedBreed)
    {
        BlockBreed genBreed;

        while(true)
        {
            genBreed = (BlockBreed)UnityEngine.Random.Range(0, 6);

            if (notAllowedBreed == genBreed)
                continue;

            break;
        }

        block.breed = genBreed;
    }

    public float CalcInitX(float offset = 0)
    {
        return -col / 2.0f + offset;
    }

    public float CalcInitY(float offset = 0)
    {
        return -row / 2.0f + offset;
    }

    public bool IsSwipeable(int nRow, int nCol)
    {
        return cells[nRow, nCol].GetCellType.IsBlockMovableType();
    }

    public IEnumerator Evaluate(Returnable<bool> matchResult)
    {
        
        // 1. 모든 블럭의 매칭 정보(개수, 상태, 내구도 등)를 계산한 후, 3Match 블럭이 있으면 true를 리턴
        bool bMathedBlockFound = UpdateAllBlocksMatchedStatus();

        // 2. 3 매치 블럭이 없는 경우
        if (bMathedBlockFound == false)
        {
            matchResult.value = false;
            yield break;
        }

        // 3. 3 매치 블럭 있는 경우

        // 3.1 첫번째 phase
        // 매칭된 블럭에 지정된 액션을 수행한
        // ex) 가로줄의 블럭 전체가 클리어 되는 블럭인 경우에 처리 등
        for(int nRow = 0; nRow < row; nRow++)
        {
            for (int nCol = 0; nCol < col; nCol++)
            {
                Block block = blocks[nRow, nCol];
                block?.DoEvaluation(mBoardEnumerator, nRow, nCol);
            }
        }
        // 3.2 두번째 phase
        // 첫번째 phase에서 반영된 블럭의 상태값에 따라서 블럭의 최종 상태를 반영한
        List<Block> clearBlocks = new List<Block>();

        for (int nRow = 0; nRow < row; nRow++)
        {
            for (int nCol = 0; nCol < col; nCol++)
            {
                Block block = blocks[nRow, nCol];

                if(block != null)
                {
                    if(block.status == BlockStatus.CLEAR)
                    {
                        clearBlocks.Add(block);
                        blocks[nRow, nCol] = null;
                    }
                }
            }
        }

        // 3.3 매칭된 블럭을 제거한다.
        clearBlocks.ForEach(block => block.Destroy());
        // 3.4 3 매치 블럭 있는 경우 true 설정
        matchResult.value = true;

        yield break;
    }
    public bool UpdateAllBlocksMatchedStatus()
    {
        List<Block> MatchedBlockList = new List<Block>();
        int nCount = 0;
        for(int nRow = 0; nRow < row; nRow++)
        {
            for(int nCol = 0; nCol < col; nCol++)
            {
                if(EvalBlocksIfMatched(nRow, nCol, MatchedBlockList))
                {
                    nCount++;
                }
            }
        }

        return nCount > 0;
    }
    public bool EvalBlocksIfMatched(int nRow, int nCol, List<Block> matchedBlockList)
    {
        bool bFound = false;

        Block baseBlock = blocks[nRow, nCol];
        if (baseBlock == null)
            return false;
        if (baseBlock.match != MatchType.NONE || !baseBlock.isValidate() || cells[nRow, nCol].IsObstracle())
            return false;
        // 검사하는 자신을 매칭 리스트에 우선 보관한다.
        matchedBlockList.Add(baseBlock);

        // 1. 가로 블럭 검색
        Block block;

        // 1.1 오른쪽 방향
        for(int i = nCol+1; i < col; i++)
        {
            block = blocks[nRow, i];
            // 자신 블럭과 오른쪽 블럭이 맞지 않으면 break;
            if (!block.IsSafeEqual(baseBlock))
                break;

            matchedBlockList.Add(block);
        }

        // 1.2 왼쪽 방향
        for (int i = nCol - 1; i >= 0 ; i--)
        {
            block = blocks[nRow, i];
            // 자신 블럭과 오른쪽 블럭이 맞지 않으면 break;
            if (!block.IsSafeEqual(baseBlock))
                break;

            matchedBlockList.Insert(0, block);
        }
        // 1.3 매치된 상태인지 판단한다
        // 기준 블럭(baseBlock)을 제외하고 좌우에 2개이상이면 기준블럭 포함해서 3개이상 매치되는 경우로 판단됨!
        if (matchedBlockList.Count >= 3)
        {
            SetBlockStatusMatched(matchedBlockList, true);
            bFound = true;
        }

        matchedBlockList.Clear();

        // 2. 세로 블럭 검색
        matchedBlockList.Add(baseBlock);

        // 2.1 위쪽 방향
        for (int i = nRow + 1; i < row; i++)
        {
            block = blocks[i, nCol];
            // 자신 블럭과 오른쪽 블럭이 맞지 않으면 break;
            if (!block.IsSafeEqual(baseBlock))
                break;

            matchedBlockList.Add(block);
        }

        // 2.2 아래쪽 방향
        for (int i = nRow - 1; i >= 0; i--)
        {
            block = blocks[i, nCol];
            // 자신 블럭과 오른쪽 블럭이 맞지 않으면 break;
            if (!block.IsSafeEqual(baseBlock))
                break;

            matchedBlockList.Insert(0, block);
        }

        // 2.3 매치된 상태인지 판단한다
        // 기준 블럭(baseBlock)을 제외하고 좌우에 2개이상이면 기준블럭 포함해서 3개이상 매치되는 경우로 판단됨!
        if (matchedBlockList.Count >= 3)
        {
            SetBlockStatusMatched(matchedBlockList, false);
            bFound = true;
        }
        
        // 계산위해 리스트에 저장한 블럭 제거
        matchedBlockList.Clear();

        return bFound;
    }

    private void SetBlockStatusMatched(List<Block> blockList, bool bHorz)
    {

        int nMatchCount = blockList.Count;
        blockList.ForEach(block => block.UpdateBlockStatusMatched((MatchType)nMatchCount));
    }
}
