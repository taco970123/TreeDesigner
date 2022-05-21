using System.Collections.Generic;

namespace TreeDesigner.Runtime
{
    [NodeName("StringList")]
    [NodePath("Base/Value/StringListNode")]
    public class StringListNode : ValueNode
    {
        [PortInfo("FloatList", 1, typeof(List<float>))]
        public List<string> stringList = new List<string>();
    }
}
