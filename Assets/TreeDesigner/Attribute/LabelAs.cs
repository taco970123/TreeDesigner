using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class LabelAs : Attribute
{
    protected string label;
    public string Label => label;

    public LabelAs(string label)
    {
        this.label = label;
    }
}
