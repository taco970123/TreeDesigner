namespace TreeDesigner.Runtime
{
    [NodeName("Divide")]
    [NodePath("Base/Operate/Math/DivideNode")]
    public class DivideNode : OperateNode
    {
        [PortInfo("Float1", 0, typeof(float))]
        public float float1;
        [PortInfo("Float2", 0, typeof(float))]
        public float float2;
        [PortInfo("Float", 1, typeof(float))]
        public float float3;

        protected override void Operate()
        {
            float3 = float1 / float2;
        }
    }
}