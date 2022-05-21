using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("Clamp")]
    [NodePath("Base/Operate/Math/ClampNode")]
    public class ClampNode : OperateNode
    {
        [PortInfo("Input", 0, typeof(float))]
        public float inputValue;
        [PortInfo("Min", 0, typeof(float))]
        public float minValue;
        [PortInfo("Max", 0, typeof(float))]
        public float maxValue;
        [PortInfo("Output", 1, typeof(float)), ReadOnly]
        public float outputValue;

        protected override void Operate()
        {
            outputValue = Mathf.Clamp(inputValue, minValue, maxValue);
        }
    }
}
