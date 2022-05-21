using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("Max")]
    [NodePath("Base/Operate/Math/MaxNode")]
    public class MaxNode : OperateNode
    {
        [PortInfo("Float1", 0, typeof(float))]
        public float float1Value;
        [PortInfo("Float2", 0, typeof(float))]
        public float float2Value;
        [PortInfo("Float", 1, typeof(float)), ReadOnly]
        public float floatValue;

        protected override void Operate()
        {
            floatValue = Mathf.Max(float1Value, float2Value);
        }
    }
}
