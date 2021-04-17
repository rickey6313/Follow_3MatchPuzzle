
public class BoardEnumerator
{
    Board mBoard;

    public BoardEnumerator(Board board)
    {
        mBoard = board;
    }

    /// <summary>
    /// 나중에 추가될 철장 타입의 셀인지 검사
    /// </summary>
    /// <param name="nRow"></param>
    /// <param name="nCol"></param>
    /// <returns></returns>
    public bool IsCageTypeCell(int nRow, int nCol)
    {
        return false;
    }
}
