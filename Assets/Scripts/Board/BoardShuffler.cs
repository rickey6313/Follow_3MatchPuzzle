using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    using BlockVectorKV = KeyValuePair<Block, Vector2Int>;

    public class BoardShuffler
    {
        private Board mBoard;
        private bool bLoadingMode;

        // ���� ���� ���� ����ϴ� ����Ʈ
        private SortedList<int, BlockVectorKV> mOrgBlocks = new SortedList<int, BlockVectorKV>();
        // SortedList ������ ���� IEnumerator
        private IEnumerator<KeyValuePair<int, BlockVectorKV>> mIt;
        // ��ġ ���� �� 3 ��ġ �߻��� ���� �ӽ� �����ϴ� ť
        private Queue<BlockVectorKV> mUnusedBlocks = new Queue<BlockVectorKV>();

        /// <summary>
        /// SortedList�� ��ȸ�� ���� ���������� true, ��ȸ�� ��� ��ġ�� ť���� ���� �ִ� ���� 
        /// ó���ϴ� �����̸� false
        /// </summary>
        private bool bListComplete;

        public BoardShuffler(Board _board, bool _LoadingMode)
        {
            mBoard = _board;
            bLoadingMode = _LoadingMode;
        }

        public void Shuffle(bool bAnimation = false)
        {
            // 1. ���� �������� �� ���� ��Ī ������ ������Ʈ�Ѵ�
            PrepareDuplicationDatas();
            // 2. ���� ��� ���� ���� ����Ʈ�� �����Ѵ�
            PrepareShuffleBlocks();
            // 3. 1, 2���� �غ��� �����͸� �̿��Ͽ� ������ �����Ѵ�.
            Runshuffle(bAnimation);
        }

        private void PrepareDuplicationDatas()
        {
            for(int nRow = 0; nRow <mBoard.maxRow; nRow++)
            {
                for (int nCol = 0; nCol < mBoard.maxCol; nCol++)
                {
                    Block block = mBoard.GetBlocks[nRow, nCol];

                    if (block == null)
                        continue;

                    if (mBoard.CanShuffle(nRow, nCol, bLoadingMode))
                        block.ResetDuplicationInfo();
                    // �������� ���ϴ� ���� ��Ī ������ ����Ѵ�.
                    else
                    {
                        block.horzDuplicate = 1;
                        block.vertDuplicate = 1;

                        //���� ��ġ�� ���� �̴��(��, �������� ���ϴ� ��)�� ���� ��ġ ���¸� �ݿ��Ѵ�
                        //(3���̻� ��ġ�Ǵ� ���� �߻����� �ʱ� ������ ������ ���� �˻��ϸ� �ȴ�)
                        //Note : ���ϸ� ����ص� ��ü ���� ��� �˻��� �� �ִ�.

                        // ������ �����ϸ鼭 ���ÿ� �� ��ǥ ���� ������ ���� �������� üũ (�Ϳ�..)
                        if(nCol > 0 && !mBoard.CanShuffle(nRow, nCol -1, bLoadingMode) && mBoard.GetBlocks[nRow,nCol - 1].IsSafeEqual(block))
                        {
                            // ������ Dulicate++
                            block.horzDuplicate = 2;
                            mBoard.GetBlocks[nRow, nCol - 1].horzDuplicate = 2;
                        }
                        // ������ �����ϸ鼭 ���ÿ� �� ��ǥ ���� ������ ���� �������� üũ
                        if (nRow > 0 && !mBoard.CanShuffle(nRow-1,nCol, bLoadingMode) && mBoard.GetBlocks[nRow-1,nCol].IsSafeEqual(block))
                        {
                            // ������ Dulicate++
                            block.vertDuplicate = 2;
                            mBoard.GetBlocks[nRow - 1, nCol].vertDuplicate = 2;
                        }
                    }
                }
            }
        }

        private void PrepareShuffleBlocks()
        {
            for (int nRow = 0; nRow < mBoard.maxRow; nRow++)
            {
                for (int nCol = 0; nCol < mBoard.maxCol; nCol++)
                {
                    if (!mBoard.CanShuffle(nRow, nCol, bLoadingMode))
                        continue;

                    // SortedList�� ������ ���ϱ� ���ؼ� �ߺ����� ������ ���� ���� ������ �� Ű������ �����Ѵ�.
                    while(true)
                    {
                        int nRandom = Random.Range(0, 10000);
                        // detect key duplication
                        if (mOrgBlocks.ContainsKey(nRandom))
                            continue;

                        mOrgBlocks.Add(nRandom, new BlockVectorKV(mBoard.GetBlocks[nRow, nCol], new Vector2Int(nCol, nRow)));
                        break;
                    }
                }
            }
            mIt = mOrgBlocks.GetEnumerator();
        }

        private void Runshuffle(bool bAnimation)
        {
            for (int nRow = 0; nRow < mBoard.maxRow; nRow++)
            {
                for (int nCol = 0; nCol < mBoard.maxCol; nCol++)
                {
                    // 1. ���� �̴�� ���� Pass
                    if (!mBoard.CanShuffle(nRow, nCol, bLoadingMode))
                        continue;

                    // 2. ���� ��� ���� ���� ��ġ�� ���� ���� �޾Ƽ� �����Ѵ�.
                    mBoard.GetBlocks[nRow, nCol] = GetShuffledBlock(nRow, nCol);
                }
            }
        }

        private Block GetShuffledBlock(int nRow, int nCol)
        {
            // ó�� �񱳽ÿ� ������ ����
            BlockBreed preBreed = BlockBreed.NA;
            // ����Ʈ�� ���� ó���ϰ� ť�� ���� ��쿡 �ߺ� üũ ���� ���(ť���� ���� ù��° ��)            
            Block firstBlock = null;

            // true : ť���� ����, false : ����Ʈ���� ����
            bool bUseQueue = true;

            while(true)
            {
                // 1. Queue���� ���� �ϳ� ������. ù��° �ĺ���.
                BlockVectorKV blockInfo = NextBlock(bUseQueue);
                Block block = blockInfo.Key;

                // 2. ����Ʈ���� ���� ���� ó���� ��� : ��ü �������� 1ȸ�� �߻�
                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;                
                }

                Debug.Assert(block != null, $"block can't be null : queue cout -> {mUnusedBlocks.Count}");
                // ù �񱳽� ���� ����
                if (preBreed == BlockBreed.NA)
                    preBreed = block.breed;

                // 3. ����Ʈ�� ��� ó���� ���
                if(bListComplete)
                {
                    if(firstBlock == null)
                    {
                        // 3.1 ��ü ����Ʈ�� ó���ϰ�, ó������ ť���� ���� ���
                        // ť���� ���� ù��° ��
                        firstBlock = block;
                    }
                    else if(System.Object.ReferenceEquals(firstBlock, block))
                    {
                        // 3.2 ó�� ���Ҵ� ���� �ٽ� ó���ϴ� ���
                        // ��, ť�� ����ִ� ��� ���� ���ǿ� ���� �ʴ� ��� (���� �� �߿� ���ǿ� �´°� ���� ���)
                        mBoard.ChangeBlock(block, preBreed);
                    }
                }

                // 4. �����¿� ���� ���� ��ġ�� ������ ����Ѵ�.
                Vector2Int vtDup = CalcDuplications(nRow, nCol, block);

                // 5. 2�� �̻� ��ġ�Ǵ� ���, ���� ��ġ�� �ش� ���� �� �� �����Ƿ� ť�� �����ϰ� ���� �� ó���ϵ��� continue�Ѵ�
                if(vtDup.x > 2 || vtDup.y > 2)
                {
                    mUnusedBlocks.Enqueue(blockInfo);
                    bUseQueue = bListComplete || !bUseQueue;

                    continue;
                }

                // 6. ���� ��ġ�� �� �ִ� ���, ã�� ��ġ�� block GameObject�� �̵���Ų��.
                block.vertDuplicate = vtDup.y;
                block.horzDuplicate = vtDup.x;

                if(block.blockObj != null)
                {
                    float initX = mBoard.CalcInitX(Constants.BLOCK_ORG);
                    float initY = mBoard.CalcInitY(Constants.BLOCK_ORG);
                    block.Move(initX + nCol, initY + nRow);
                }

                // 7. ã�� ���� �����Ѵ�.
                return block;
            }
        }

        private BlockVectorKV NextBlock(bool bUseQueue)
        {
            // ť���� ����
            if (bUseQueue && mUnusedBlocks.Count > 0)
                return mUnusedBlocks.Dequeue();

            // SortedList���� ����
            if (!bListComplete && mIt.MoveNext())
                return mIt.Current.Value;

            bListComplete = true;

            return new BlockVectorKV(null, Vector2Int.zero);
        }

        /// <summary>
        /// ���� �������� �̹� ���� ��ġ �Ϸ�� ���� ���� ��ġ�� �� ���� ���ӹ�ġ ������ ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="block"></param>
        private Vector2Int CalcDuplications(int nRow, int nCol, Block block)
        {
            int colDup = 1, rowDup = 1;

            if (nCol > 0 && mBoard.GetBlocks[nRow, nCol - 1].IsSafeEqual(block))
                colDup += mBoard.GetBlocks[nRow, nCol - 1].horzDuplicate;

            if (nRow > 0 && mBoard.GetBlocks[nRow -1, nCol].IsSafeEqual(block))
                rowDup += mBoard.GetBlocks[nRow - 1, nCol].vertDuplicate;

            if(nCol < mBoard.maxCol -1 && mBoard.GetBlocks[nRow, nCol + 1].IsSafeEqual(block))
            {
                Block rightBlock = mBoard.GetBlocks[nRow, nCol + 1];
                colDup += rightBlock.horzDuplicate;

                // ���� �̴�� ���� ���� ���� �ߺ��Ǵ� ���, ���ù̴�� ���� �ߺ� ������ �Բ� ������Ʈ�Ѵ�.
                if (rightBlock.horzDuplicate == 1)
                    rightBlock.horzDuplicate = 2;
            }

            if (nRow < mBoard.maxRow - 1 && mBoard.GetBlocks[nRow + 1, nCol].IsSafeEqual(block))
            {
                Block upBlock = mBoard.GetBlocks[nRow + 1, nCol];
                rowDup += upBlock.vertDuplicate;

                // ���� �̴�� ���� ���� ���� �ߺ��Ǵ� ���, ���ù̴�� ���� �ߺ� ������ �Բ� ������Ʈ�Ѵ�.
                if (upBlock.vertDuplicate == 1)
                    upBlock.vertDuplicate = 2;
            }

            return new Vector2Int(colDup, rowDup);
        }
    }
}
