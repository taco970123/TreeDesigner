using System.Collections.Generic;

namespace TreeDesigner.Runtime
{
    [NodeName("FloatList")]
    [NodePath("Base/Value/FloatListNode")]
    public class FloatListNode : ValueNode
    {
        [PortInfo("FloatList", 1, typeof(List<float>))]
        public List<float> floatList;
    }
}