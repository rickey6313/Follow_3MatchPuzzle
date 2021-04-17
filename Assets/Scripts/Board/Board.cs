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
        
        // Shuffler ���� �� ����
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
        
        // 1. ��� ���� ��Ī ����(����, ����, ������ ��)�� ����� ��, 3Match ���� ������ true�� ����
        bool bMathedBlockFound = UpdateAllBlocksMatchedStatus();

        // 2. 3 ��ġ ���� ���� ���
        if (bMathedBlockFound == false)
        {
            matchResult.value = false;
            yield break;
        }

        // 3. 3 ��ġ �� �ִ� ���

        // 3.1 ù��° phase
        // ��Ī�� ���� ������ �׼��� ������
        // ex) �������� �� ��ü�� Ŭ���� �Ǵ� ���� ��쿡 ó�� ��
        for(int nRow = 0; nRow < row; nRow++)
        {
            for (int nCol = 0; nCol < col; nCol++)
            {
                Block block = blocks[nRow, nCol];
                block?.DoEvaluation(mBoardEnumerator, nRow, nCol);
            }
        }
        // 3.2 �ι�° phase
        // ù��° phase���� �ݿ��� ���� ���°��� ���� ���� ���� ���¸� �ݿ���
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

        // 3.3 ��Ī�� ���� �����Ѵ�.
        clearBlocks.ForEach(block => block.Destroy());

        // 3.3.1 ���� ���ŵǴ� ���� ��� Dleay, �� ���Ű� ���İ��� �Ͼ�� �Ϳ� �ణ ������ ��Ŵ
        yield return new WaitForSeconds(0.15f);

        // 3.4 3 ��ġ �� �ִ� ��� true ����
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
        // �˻��ϴ� �ڽ��� ��Ī ����Ʈ�� �켱 �����Ѵ�.
        matchedBlockList.Add(baseBlock);

        // 1. ���� �� �˻�
        Block block;

        // 1.1 ������ ����
        for(int i = nCol+1; i < col; i++)
        {
            block = blocks[nRow, i];
            // �ڽ� ���� ������ ���� ���� ������ break;
            if (!block.IsSafeEqual(baseBlock))
                break;

            matchedBlockList.Add(block);
        }

        // 1.2 ���� ����
        for (int i = nCol - 1; i >= 0 ; i--)
        {
            block = blocks[nRow, i];
            // �ڽ� ���� ������ ���� ���� ������ break;
            if (!block.IsSafeEqual(baseBlock))
                break;

            matchedBlockList.Insert(0, block);
        }
        // 1.3 ��ġ�� �������� �Ǵ��Ѵ�
        // ���� ��(baseBlock)�� �����ϰ� �¿쿡 2���̻��̸� ���غ� �����ؼ� 3���̻� ��ġ�Ǵ� ���� �Ǵܵ�!
        if (matchedBlockList.Count >= 3)
        {
            SetBlockStatusMatched(matchedBlockList, true);
            bFound = true;
        }

        matchedBlockList.Clear();

        // 2. ���� �� �˻�
        matchedBlockList.Add(baseBlock);

        // 2.1 ���� ����
        for (int i = nRow + 1; i < row; i++)
        {
            block = blocks[i, nCol];
            // �ڽ� ���� ������ ���� ���� ������ break;
            if (!block.IsSafeEqual(baseBlock))
                break;

            matchedBlockList.Add(block);
        }

        // 2.2 �Ʒ��� ����
        for (int i = nRow - 1; i >= 0; i--)
        {
            block = blocks[i, nCol];
            // �ڽ� ���� ������ ���� ���� ������ break;
            if (!block.IsSafeEqual(baseBlock))
                break;

            matchedBlockList.Insert(0, block);
        }

        // 2.3 ��ġ�� �������� �Ǵ��Ѵ�
        // ���� ��(baseBlock)�� �����ϰ� �¿쿡 2���̻��̸� ���غ� �����ؼ� 3���̻� ��ġ�Ǵ� ���� �Ǵܵ�!
        if (matchedBlockList.Count >= 3)
        {
            SetBlockStatusMatched(matchedBlockList, false);
            bFound = true;
        }
        
        // ������� ����Ʈ�� ������ �� ����
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

            // 1. ���� ��(col)�� �� ���� �����Ѵ�.
            // ���� col�� �ٸ� row�� ����ִ� �� �ε����� �����Ѵ�. SortedList�̹Ƿ� ù��° ��尡 ���� �Ʒ��� �� ��ġ�̴�.
            for(int nRow = 0; nRow < row; nRow++)
            {
                if (CanBlockBeAllocatable(nRow, nCol))
                {
                    emptyBlocks.Add(nRow, nRow);
                }
                    
            }
            // �Ʒ��ʿ� ����ִ� ���� ���� ���
            if (emptyBlocks.Count == 0)
                continue;

            // 2. �̵��� ������ ���� ����ִ� �ϴ� ��ġ�� �̵��Ѵ�.

            // 2.1 ���� �Ʒ��ʺ��� ����ִ� ���� ó���Ѵ�.
            IntIntKV first = emptyBlocks.First();

            // 2.2 ����ִ� �� ���� �������� �̵� ������ ���� Ž���ϸ鼭 �� ���� ä��������
            for(int nRow = first.Value + 1; nRow < row; nRow++)
            {
                Block block = blocks[nRow, nCol];

                // 2.2.1 �̵� ������ �������� �ƴ� ��� pass
                if (block == null || cells[nRow, nCol].GetCellType == CellType.EMPTY)
                    continue;

                // ����� �����ϴ� cell�� �ִ� ��� �ش� cell �Ʒ����� ����д�
                // �Ʒ��� ����ִ� ���� �̵������� ��Ͽ��� �����Ѵ�

                // 2.2.2 �̵��� �ʿ��� �� �߰�                
                block.dropDistance = new Vector2(0, nRow - first.Value);
                movingBlocks.Add(block);

                // 2.2.3 �� �������� �̵�
                Debug.Assert(cells[first.Value, nCol].IsObstracle() == false, $"{cells[first.Value, nCol]}");
                blocks[first.Value, nCol] = block;

                // 2.2.4 �ٸ� ������ �̵������Ƿ� ���� ��ġ�� ����д�
                blocks[nRow, nCol] = null;

                // 2.2.5 ����ִ� �� ����Ʈ���� ���� ù��° ���(first)�� �����Ѵ�
                emptyBlocks.RemoveAt(0);

                // 2.2.6 ���� ��ġ�� ���� �ٸ� ��ġ�� �̵������Ƿ� ���� ��ġ�� ����ְ� �ȴ�.
                // �׷��Ƿ� ����ִ� ���� �����ϴ� emptyBlocks�� �߰��Ѵ�
                emptyBlocks.Add(nRow, nRow);

                // 2.2.7 ���� ����ִ� ���� ó���ϵ��� ������ �����Ѵ�
                first = emptyBlocks.First();
                nRow = first.Value;

            }
        }

        yield return null;

        // ������� ä������ �ʴ� ���� �ִ� ���(���� �Ʒ� ������ �������)
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
                // ����ִ� ���� �ִ� ���, ���� ���� ��� ����ְų�, ��ֹ��� ���ؼ� ��������
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
    /// ���� �����ϰ� ������(nRow, nCol)���� ����Ѵ�
    /// </summary>
    /// <param name="nRow">��� �� ��ġ X</param>
    /// <param name="nCol">��� �� ��ġ Y</param>
    /// <param name="nSpawnRow">��� �� ��ġ X</param>
    /// <param name="nSpawnCol">��� �� ��ġ Y</param>
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