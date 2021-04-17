using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;
using IntIntKV = System.Collections.Generic.KeyValuePair<int, int>;

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

        // 3.3.1 블럭이 제거되는 동안 잠시 Dleay, 블럭 제거가 순식간에 일어나는 것에 약간 지연을 시킴
        yield return new WaitForSeconds(0.15f);

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

    public IEnumerator ArrangeBlocksAfterClean(List<IntIntKV> unfilledBlocks, List<Block> movingBlocks)
    {
        SortedList<int, int> emptyBlocks = new SortedList<int, int>();
        List<IntIntKV> emptyRemainBlocks = new List<IntIntKV>();

        for(int nCol = 0; nCol < col; nCol++)
        {
            emptyBlocks.Clear();

            // 1. 같은 열(col)에 빈 블럭을 수집한다.
            // 현재 col의 다른 row의 비어있는 블럭 인덱스를 수집한다. SortedList이므로 첫번째 노드가 가장 아래쪽 블럭 위치이다.
            for(int nRow = 0; nRow < row; nRow++)
            {
                if (CanBlockBeAllocatable(nRow, nCol))
                {
                    emptyBlocks.Add(nRow, nRow);
                }
                    
            }
            // 아래쪽에 비어있는 블럭이 없는 경우
            if (emptyBlocks.Count == 0)
                continue;

            // 2. 이동이 가능한 블럭을 비어있는 하단 위치로 이동한다.

            // 2.1 가장 아래쪽부터 비어있는 블럭을 처리한다.
            IntIntKV first = emptyBlocks.First();

            // 2.2 비어있는 블럭 위쪽 방향으로 이동 가능한 블럭을 탐색하면서 빈 블럭을 채워나간다
            for(int nRow = first.Value + 1; nRow < row; nRow++)
            {
                Block block = blocks[nRow, nCol];

                // 2.2.1 이동 가능한 아이템이 아닌 경우 pass
                if (block == null || cells[nRow, nCol].GetCellType == CellType.EMPTY)
                    continue;

                // 드롭을 방해하는 cell이 있는 경우 해당 cell 아래쪽은 비워둔다
                // 아래쪽 비어있는 블럭은 이동가능한 목록에서 제거한다

                // 2.2.2 이동이 필요한 블럭 발견                
                block.dropDistance = new Vector2(0, nRow - first.Value);
                movingBlocks.Add(block);

                // 2.2.3 빈 공간으로 이동
                Debug.Assert(cells[first.Value, nCol].IsObstracle() == false, $"{cells[first.Value, nCol]}");
                blocks[first.Value, nCol] = block;

                // 2.2.4 다른 곳으로 이동했으므로 현재 위치는 비워둔다
                blocks[nRow, nCol] = null;

                // 2.2.5 비어있는 블럭 리스트에서 사용된 첫번째 노드(first)를 삭제한다
                emptyBlocks.RemoveAt(0);

                // 2.2.6 현재 위치의 블럭이 다른 위치로 이동했으므로 현재 위치가 비어있게 된다.
                // 그러므로 비어있는 블럭을 보관하는 emptyBlocks에 추가한다
                emptyBlocks.Add(nRow, nRow);

                // 2.2.7 다음 비어있는 블럭을 처리하도록 기준을 변경한다
                first = emptyBlocks.First();
                nRow = first.Value;

            }
        }

        yield return null;

        // 드롭으로 채워지지 않는 블럭이 있는 경우(왼쪽 아래 순으로 들어있음)
        if (emptyRemainBlocks.Count > 0)
            unfilledBlocks.AddRange(emptyRemainBlocks);

        yield break;
    }

    public IEnumerator SpawnBlocksAfterClean(List<Block> movingBlocks)
    {
        for(int nCol = 0; nCol < col; nCol++)
        {
            for (int nRow = 0; nRow < row; nRow++)
            {
                // 비어있는 블럭이 있는 경우, 상위 열은 모두 비어있거나, 장애물로 인해서 남아있음
                if(blocks[nRow, nCol] == null)
                {
                    int nTopRow = nRow;

                    int nSpawnBaseY = 0;
                    for(int y = nTopRow; y < row; y++)
                    {
                        if (blocks[y, nCol] != null || !CanBlockBeAllocatable(y, nCol))
                            continue;

                        Block block = SpawnBlockWithDrop(y, nCol, nSpawnBaseY, nCol);
                        if (block != null)
                            movingBlocks.Add(block);
                        nSpawnBaseY++;
                    }

                    break;
                }
            }
        }
        yield return null;
    }

    private bool CanBlockBeAllocatable(int nRow, int nCol)
    {
        if (!cells[nRow, nCol].GetCellType.IsBlockAllocatableType())
            return false;

        return blocks[nRow, nCol] == null;
    }

    /// <summary>
    /// 블럭을 생성하고 목적지(nRow, nCol)까지 드롭한다
    /// </summary>
    /// <param name="nRow">드롭 후 위치 X</param>
    /// <param name="nCol">드롭 후 위치 Y</param>
    /// <param name="nSpawnRow">드롭 전 위치 X</param>
    /// <param name="nSpawnCol">드롭 전 위치 Y</param>
    /// <returns></returns>
    private Block SpawnBlockWithDrop(int nRow, int nCol, int nSpawnRow, int nSpawnCol)
    {
        float fInitX = CalcInitX(Constants.BLOCK_ORG);
        float fInitY = CalcInitY(Constants.BLOCK_ORG) + row;

        Block block = stageBuilder.SpawnBlock().InstantiateBlockObj(blockPrefab, container);
        if(block != null)
        {
            blocks[nRow, nCol] = block;
            block.Move(fInitX + (float)nSpawnCol, fInitY + (float)nSpawnRow);
            block.dropDistance = new Vector2(nSpawnCol - nCol, row + (nSpawnRow - nRow));
        }

        return block;
    }
}