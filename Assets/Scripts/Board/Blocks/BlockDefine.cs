
public enum BlockType
{
    EMPTY = 0,
    BASIC = 1
}

public enum BlockBreed
{
    NA = -1,
    BREED_0 = 0,
    BREED_1 = 1,
    BREED_2 = 2,
    BREED_3 = 3,
    BREED_4 = 4,
    BREED_5 = 5,
}

public enum BlockStatus
{
    /// <summary>
    /// 기본상태
    /// </summary>
    NORMAL,
    /// <summary>
    /// 매칭 블럭 있는 상태
    /// </summary>    
    MATCH,
    /// <summary>
    /// 클리어 예정 상태
    /// </summary>
    CLEAR
}

public enum BlockQuestType
{ 
    NONE = -1,              
    CLEAR_SIMPLE = 0,       // 단일 블럭 제거
    CLEAR_HORZ = 1,         // 세로줄 블럭 제거 -> 4 match 가로형
    CLEAR_VERT = 2,         // 가로줄 블럭 제거
    CLEAR_CIRCLE = 3,       // 인접한 주변영역 블럭 제거 -> T L 매치 ( 3*3, 4*3)
    CLEAR_LAZER = 4,        // 지정된 블럭과 동일한 블럭 전체 제거 -> 5 match
    CLEAR_HORZ_BUFF = 5,    // HORZ + CIRCLE 조합
    CLEAR_VERT_BUFF = 6,    // VERT + CIRCLE 조합
    CLEAR_CIRCLE_BUFF = 7,  // CIRCLE + CIRCLE 조합
    CLEAR_LAZER_BUFF = 8    // LAZER + LAZER 조합
}


static class BlockTypeMethod
{
    public static bool IsSafeEqual(this Block block, Block targetBlock)
    {
        if (block == null)
            return false;

        return block.IsEqual(targetBlock);
    }
}