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

        // 블럭을 섞기 위해 사용하는 리스트
        private SortedList<int, BlockVectorKV> mOrgBlocks = new SortedList<int, BlockVectorKV>();
        // SortedList 꺼내기 위한 IEnumerator
        private IEnumerator<KeyValuePair<int, BlockVectorKV>> mIt;
        // 배치 과정 중 3 매치 발생된 블럭을 임시 보관하는 큐
        private Queue<BlockVectorKV> mUnusedBlocks = new Queue<BlockVectorKV>();

        /// <summary>
        /// SortedList에 조회할 블럭이 남아있으면 true, 조회를 모두 마치고 큐에서 남아 있는 블럭을 
        /// 처리하는 과정이면 false
        /// </summary>
        private bool bListComplete;

        public BoardShuffler(Board _board, bool _LoadingMode)
        {
            mBoard = _board;
            bLoadingMode = _LoadingMode;
        }

        public void Shuffle(bool bAnimation = false)
        {
            // 1. 셔플 시작전에 각 블럭의 매칭 정보를 업데이트한다
            PrepareDuplicationDatas();
            // 2. 셔플 대상 블럭을 별도 리스트에 보관한다
            PrepareShuffleBlocks();
            // 3. 1, 2에서 준비한 데이터를 이용하여 셔플을 수행한다.
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
                    // 움직이지 못하는 블럭의 매칭 정보를 계산한다.
                    else
                    {
                        block.horzDuplicate = 1;
                        block.vertDuplicate = 1;

                        //좌하 위치에 셔플 미대상(즉, 움직이지 못하는 블럭)인 블럭의 매치 상태를 반영한다
                        //(3개이상 매치되는 경우는 발생하지 않기 때문에 인접한 블럭만 검사하면 된다)
                        //Note : 좌하만 계산해도 전체 블럭을 모두 검사할 수 있다.

                        // 셔플이 가능하면서 동시에 내 좌표 블럭과 좌측의 블럭과 같은건지 체크 (와우..)
                        if(nCol > 0 && !mBoard.CanShuffle(nRow, nCol -1, bLoadingMode) && mBoard.GetBlocks[nRow,nCol - 1].IsSafeEqual(block))
                        {
                            // 같으면 Dulicate++
                            block.horzDuplicate = 2;
                            mBoard.GetBlocks[nRow, nCol - 1].horzDuplicate = 2;
                        }
                        // 셔플이 가능하면서 동시에 내 좌표 블럭과 하측의 블럭과 같은건지 체크
                        if (nRow > 0 && !mBoard.CanShuffle(nRow-1,nCol, bLoadingMode) && mBoard.GetBlocks[nRow-1,nCol].IsSafeEqual(block))
                        {
                            // 같으면 Dulicate++
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

                    // SortedList에 순서를 정하기 위해서 중복값이 없도록 랜덤 값을 생성한 후 키값으로 저장한다.
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
                    // 1. 셔플 미대상 블럭은 Pass
                    if (!mBoard.CanShuffle(nRow, nCol, bLoadingMode))
                        continue;

                    // 2. 셔플 대상 블럭은 새로 배치할 블럭을 리턴 받아서 저장한다.
                    mBoard.GetBlocks[nRow, nCol] = GetShuffledBlock(nRow, nCol);
                }
            }
        }

        private Block GetShuffledBlock(int nRow, int nCol)
        {
            // 처음 비교시에 종류를 저장
            BlockBreed preBreed = BlockBreed.NA;
            // 리스트를 전부 처리하고 큐만 남은 경우에 중복 체크 위해 사용(큐에서 꺼낸 첫번째 블럭)            
            Block firstBlock = null;

            // true : 큐에서 꺼냄, false : 리스트에서 꺼냄
            bool bUseQueue = true;

            while(true)
            {
                // 1. Queue에서 블럭을 하나 꺼낸다. 첫번째 후보다.
                BlockVectorKV blockInfo = NextBlock(bUseQueue);
                Block block = blockInfo.Key;

                // 2. 리스트에서 블럭을 전부 처리한 경우 : 전체 루프에서 1회만 발생
                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;                
                }

                Debug.Assert(block != null, $"block can't be null : queue cout -> {mUnusedBlocks.Count}");
                // 첫 비교시 종류 저장
                if (preBreed == BlockBreed.NA)
                    preBreed = block.breed;

                // 3. 리스트를 모두 처리한 경우
                if(bListComplete)
                {
                    if(firstBlock == null)
                    {
                        // 3.1 전체 리스트를 처리하고, 처음으로 큐에서 꺼낸 경우
                        // 큐에서 꺼낸 첫번째 블럭
                        firstBlock = block;
                    }
                    else if(System.Object.ReferenceEquals(firstBlock, block))
                    {
                        // 3.2 처음 보았던 블럭을 다시 처리하는 경우
                        // 즉, 큐에 들어있는 모든 블럭이 조건에 맞지 않는 경우 (남은 블럭 중에 조건에 맞는게 없는 경우)
                        mBoard.ChangeBlock(block, preBreed);
                    }
                }

                // 4. 상하좌우 인전 블럭과 겹치는 개수를 계산한다.
                Vector2Int vtDup = CalcDuplications(nRow, nCol, block);

                // 5. 2개 이상 매치되는 경우, 현재 위치에 해당 블럭이 올 수 없으므로 큐에 보관하고 다음 블럭 처리하도록 continue한다
                if(vtDup.x > 2 || vtDup.y > 2)
                {
                    mUnusedBlocks.Enqueue(blockInfo);
                    bUseQueue = bListComplete || !bUseQueue;

                    continue;
                }

                // 6. 블럭이 위치할 수 있는 경우, 찾은 위치로 block GameObject를 이동시킨다.
                block.vertDuplicate = vtDup.y;
                block.horzDuplicate = vtDup.x;

                if(block.blockObj != null)
                {
                    float initX = mBoard.CalcInitX(Constants.BLOCK_ORG);
                    float initY = mBoard.CalcInitY(Constants.BLOCK_ORG);
                    block.Move(initX + nCol, initY + nRow);
                }

                // 7. 찾은 블럭을 리턴한다.
                return block;
            }
        }

        private BlockVectorKV NextBlock(bool bUseQueue)
        {
            // 큐에서 꺼냄
            if (bUseQueue && mUnusedBlocks.Count > 0)
                return mUnusedBlocks.Dequeue();

            // SortedList에서 꺼냄
            if (!bListComplete && mIt.MoveNext())
                return mIt.Current.Value;

            bListComplete = true;

            return new BlockVectorKV(null, Vector2Int.zero);
        }

        /// <summary>
        /// 셔플 과정에서 이미 새로 배치 완료된 블럭과 현재 배치할 블럭 간의 연속배치 개수를 계산하는 메소드
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

                // 셔플 미대상 블럭이 현재 블럭과 중복되는 경우, 셔플미대상 블럭의 중복 정보도 함께 업데이트한다.
                if (rightBlock.horzDuplicate == 1)
                    rightBlock.horzDuplicate = 2;
            }

            if (nRow < mBoard.maxRow - 1 && mBoard.GetBlocks[nRow + 1, nCol].IsSafeEqual(block))
            {
                Block upBlock = mBoard.GetBlocks[nRow + 1, nCol];
                rowDup += upBlock.vertDuplicate;

                // 셔플 미대상 블럭이 현재 블럭과 중복되는 경우, 셔플미대상 블럭의 중복 정보도 함께 업데이트한다.
                if (upBlock.vertDuplicate == 1)
                    upBlock.vertDuplicate = 2;
            }

            return new Vector2Int(colDup, rowDup);
        }
    }
}
