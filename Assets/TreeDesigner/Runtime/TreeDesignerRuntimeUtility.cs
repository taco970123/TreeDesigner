public enum BDCompareType { Less, LessEqual, Equal, GrearerEqual, Greater, NotEqual }

public static class TreeDesignerRuntimeUtility
{
    /// <summary>
    /// �Ƚ���ֵ
    /// </summary>
    /// <param name="compareType">�Ƚ�����</param>
    /// <param name="a">��ǰֵ</param>
    /// <param name="b">����ֵ</param>
    /// <returns>�Ƿ����</returns>
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
