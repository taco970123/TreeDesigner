namespace TreeDesigner.Runtime
{
    [NodeName("String")]
    [NodePath("Base/Value/StringNode")]
    public class StringNode : ValueNode
    {
        [PortInfo("String", 1, typeof(string))]
        public string stringValue;
    }
}