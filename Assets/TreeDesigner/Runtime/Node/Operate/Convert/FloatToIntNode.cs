namespace TreeDesigner.Runtime
{
    [NodeName("FloatToInt")]
    [NodePath("Base/Operate/Convert/FloatToIntNode")]
    public class FloatToIntNode : OperateNode
    {
        [PortInfo("Float", 0, typeof(float))]
        public float floatValue;
        [PortInfo("Int", 1, typeof(int)), ReadOnly]
        public int intValue;

        protected override void Operate()
        {
            intValue = (int)floatValue;
        }
    }
}
