using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class ShowIf : Attribute
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
    public ShowIf(string name, params object[] conditions)
    {
        this.name = name;
        this.conditions = conditions;
    }
}
