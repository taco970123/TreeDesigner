using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class HoverInfo : Attribute
{
    protected string tooltip;

    public string Tooltip => tooltip;

    public HoverInfo(string tooltip)
    {
        this.tooltip = tooltip;
    }
}
