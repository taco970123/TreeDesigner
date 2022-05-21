namespace TreeDesigner.Runtime
{
    [NodeName("Int")]
    [NodePath("Base/Value/IntNode")]
    public class IntNode : ValueNode
    {
        [PortInfo("Int", 1, typeof(int))]
        public int intValue;
    }
}