using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class HideIf : Attribute
{
    protected string name;
    protected object[] conditions;

    public string Name => name;
    public object[] Conditions => conditions;

    /// <summary>
    /// Ŀ��ֵ��������ʱ��ʾ
    /// </summary>
    /// <param name="name">Ŀ�����</param>
    /// <param name="conditions">�����������</param>
    public HideIf(string name, params object[] conditions)
    {
        this.name = name;
        this.conditions = conditions;
    }
}
