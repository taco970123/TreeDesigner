namespace TreeDesigner.Runtime
{
    [NodeName("ToString")]
    [NodePath("Base/Operate/Convert/ToStringNode")]
    public class ToStringNode : OperateNode
    {
        [PortInfo("Value", 0, typeof(object)), ReadOnly]
        public object value;
        [PortInfo("String", 1, typeof(string)), ReadOnly]
        public string stringValue;

        protected override void Operate()
        {
            stringValue = value.ToString();
        }
    }
}
