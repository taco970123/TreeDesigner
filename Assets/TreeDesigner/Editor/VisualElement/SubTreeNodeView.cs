#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class SubTreeNodeView : NodeView
    {
        SubTree subTree => (node as SubTreeNode).subTree;

        public SubTreeNodeView(SubTreeNode node, string path) : base(node, path)
        {
        }

        public override void UpdatePort()
        {
            CreateInputPorts();
            CreateOutputPorts();
            GenerateValuePorts();
        }
        protected override void CreateInputPorts()
        {
            inputContainer.Clear();
            if(subTree && subTree.rootNode != null)
            {
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                input.portName = "";
                var connector = input.Q("connector");
                connector.style.borderTopLeftRadius = connector.style.borderTopRightRadius = connector.style.borderBottomLeftRadius = connector.style.borderBottomRightRadius = 0;
                var cap = connector.Q("cap");
                cap.style.borderTopLeftRadius = cap.style.borderTopRightRadius = cap.style.borderBottomLeftRadius = cap.style.borderBottomRightRadius = 0;
                inputContainer.Add(input);
            }
        }
        protected override void CreateOutputPorts()
        {
            outputContainer.Clear();
            if (subTree && subTree.endNode != null)
            {
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                output.portName = "";
                var connector = output.Q("connector");
                connector.style.borderTopLeftRadius = connector.style.borderTopRightRadius = connector.style.borderBottomLeftRadius = connector.style.borderBottomRightRadius = 0;
                var cap = connector.Q("cap");
                cap.style.borderTopLeftRadius = cap.style.borderTopRightRadius = cap.style.borderBottomLeftRadius = cap.style.borderBottomRightRadius = 0;
                outputContainer.Add(output);
            }
        }
        protected override void GenerateValuePorts()
        {
            if (subTree == null)
                return;
            valuePorts = new Dictionary<Port, string>();
            foreach (var item in subTree.InExposedProperties)
            {
                var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, item.Type);
                if (inputPort != null)
                {
                    inputPort.portName = item.Name;
                    inputContainer.Add(inputPort);
                    valuePorts.Add(inputPort, item.Name);
                }
            }
            foreach (var item in subTree.OutExposedProperties)
            {
                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, item.Type);
                if (outputPort != null)
                {
                    outputPort.portName = item.Name;
                    outputContainer.Add(outputPort);
                    valuePorts.Add(outputPort, item.Name);
                }
            }
            //if (subTree == null) return;
            //int index = 0;
            //node.InputPortDatas.Clear();
            //if (subTree.rootNode != null)
            //{
            //    node.InputPortDatas.Add(new NodePortData("m_InputPort", index, 0));
            //    index++;
            //}
            //foreach (var exposedProperty in subTree.InExposedProperties)
            //{
            //    node.InputPortDatas.Add(new NodePortData(exposedProperty.Name, index, 1));
            //    index++;
            //}
            //index = 0;
            //node.OutputPortDatas.Clear();
            //if (subTree.endNode != null)
            //{
            //    node.OutputPortDatas.Add(new NodePortData("m_OutputPort", index, 0));
            //    index++;
            //}
            //foreach (var outputPort in subTree.OutExposedProperties)
            //{
            //    node.OutputPortDatas.Add(new NodePortData(outputPort.Name, index, 1));
            //    index++;
            //}
        }
    }
}

#endif