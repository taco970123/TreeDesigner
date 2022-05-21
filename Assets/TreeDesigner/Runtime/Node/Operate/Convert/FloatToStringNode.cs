namespace TreeDesigner.Runtime
{
    [NodeName("FloatToString")]
    [NodePath("Base/Operate/Convert/FloatToStringNode")]
    public class FloatToStringNode : OperateNode
    {
        [PortInfo("Float", 0, typeof(float))]
        public float floatValue;
        [PortInfo("String",1,typeof(string))]
        public string stringValue;

        protected override void Operate()
        {
            stringValue = floatValue.ToString();
        }
    }
}