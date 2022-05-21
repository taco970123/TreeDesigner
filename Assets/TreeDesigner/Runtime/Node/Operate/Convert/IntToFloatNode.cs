namespace TreeDesigner.Runtime
{
    [NodeName("IntToFloat")]
    [NodePath("Base/Operate/Convert/IntToFloatNode")]
    public class IntToFloatNode : OperateNode
    {
        [PortInfo("Int", 0, typeof(int))]
        public int intValue;
        [PortInfo("Float", 1, typeof(float)), ReadOnly]
        public float floatValue;

        protected override void Operate()
        {
            floatValue = intValue;
        }
    }
}