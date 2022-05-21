public enum BDCompareType { Less, LessEqual, Equal, GrearerEqual, Greater, NotEqual }

public static class TreeDesignerRuntimeUtility
{
    /// <summary>
    /// 比较数值
    /// </summary>
    /// <param name="compareType">比较类型</param>
    /// <param name="a">当前值</param>
    /// <param name="b">给定值</param>
    /// <returns>是否符合</returns>
    public static bool Compare(BDCompareType compareType, float a, float b)
    {
        switch (compareType)
        {
            case BDCompareType.Less:
                return a < b;
            case BDCompareType.LessEqual:
                return a <= b;
            case BDCompareType.Equal:
                return a == b;
            case BDCompareType.GrearerEqual:
                return a >= b;
            case BDCompareType.Greater:
                return a > b;
            case BDCompareType.NotEqual:
                return a != b;
            default:
                return false;
        }
    }
}
