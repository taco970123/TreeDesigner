using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class ShowIf : Attribute
{
    protected string name;
    protected object[] conditions;

    public string Name => name;
    public object[] Conditions => conditions;

    /// <summary>
    /// 目标值符合条件时显示
    /// </summary>
    /// <param name="name">目标变量</param>
    /// <param name="conditions">多个满足条件</param>
    public ShowIf(string name, params object[] conditions)
    {
        this.name = name;
        this.conditions = conditions;
    }
}
