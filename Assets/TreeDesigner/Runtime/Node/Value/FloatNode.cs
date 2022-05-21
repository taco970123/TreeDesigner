namespace TreeDesigner.Runtime
{
    [NodeName("Float")]
    [NodePath("Base/Value/FloatNode")]
    public class FloatNode : ValueNode
    {
        [PortInfo("Float", 1, typeof(float))]
        public float floatValue;
    }
}