namespace TreeDesigner.Runtime
{
    [NodeName("Add")]
    [NodePath("Base/Operate/Math/AddNode")]
    public class AddNode : OperateNode
    {
        [PortInfo("Float1", 0, typeof(float))]
        public float float1;
        [PortInfo("Float2", 0, typeof(float))]
        public float float2;
        [PortInfo("Float", 1, typeof(float)), ReadOnly]
        public float float3;

        protected override void Operate()
        {
            float3 = float1 + float2;
        }
    }
}