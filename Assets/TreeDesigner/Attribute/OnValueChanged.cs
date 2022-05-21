using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class OnValueChanged : Attribute
{
    protected string name;
    public string Name => name;

    public OnValueChanged(string name)
    {
        this.name = name;
    }
}