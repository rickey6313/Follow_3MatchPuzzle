
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
    /// �⺻����
    /// </summary>
    NORMAL,
    /// <summary>
    /// ��Ī �� �ִ� ����
    /// </summary>    
    MATCH,
    /// <summary>
    /// Ŭ���� ���� ����
    /// </summary>
    CLEAR
}

public enum BlockQuestType
{ 
    NONE = -1,              
    CLEAR_SIMPLE = 0,       // ���� �� ����
    CLEAR_HORZ = 1,         // ������ �� ���� -> 4 match ������
    CLEAR_VERT = 2,         // ������ �� ����
    CLEAR_CIRCLE = 3,       // ������ �ֺ����� �� ���� -> T L ��ġ ( 3*3, 4*3)
    CLEAR_LAZER = 4,        // ������ ���� ������ �� ��ü ���� -> 5 match
    CLEAR_HORZ_BUFF = 5,    // HORZ + CIRCLE ����
    CLEAR_VERT_BUFF = 6,    // VERT + CIRCLE ����
    CLEAR_CIRCLE_BUFF = 7,  // CIRCLE + CIRCLE ����
    CLEAR_LAZER_BUFF = 8    // LAZER + LAZER ����
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