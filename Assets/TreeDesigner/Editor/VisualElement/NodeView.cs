#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class NodeView : Node
    {
        public BaseNode node;
        public Port input;
        public Port output;



        public Action<NodeView> onNodeSelected;
        public Action<NodeView> onNodeUnselected;
    
        public Action<NodeView> onMouse0Down;
        public Action<NodeView> onMouse2Down;
        public Action<NodeView> onDoubleClicked;
        public Action<NodeView> onOrderChildren;
        public Action<NodeView> onGeometryChanged;

        bool mouseDown;
        bool editing;
        double lastTime;
        
        protected Dictionary<Port, string> valuePorts;
        protected List<FieldInfo> fieldInfos = new List<FieldInfo>();
        protected Dictionary<FieldInfo, PortInfo> inputPorts = new Dictionary<FieldInfo, PortInfo>();
        protected Dictionary<FieldInfo, PortInfo> outputPorts = new Dictionary<FieldInfo, PortInfo>();

        public Dictionary<Port, string> ValuePorts => valuePorts;
        public NodeView() { }
        public NodeView(BaseNode node,string path): base(path)
        {
            this.node = node;
            
            if (!node.IsDeletable)
                capabilities = capabilities &~ Capabilities.Deletable;
            if(!node.IsSelectable)
                capabilities = capabilities & ~Capabilities.Selectable;

            SetName();
            SetColor();
            UpdatePort();
            CheckPortState();
            SetupClassed();
            UpdateState();

            viewDataKey = node.GUID;

            style.left = node.Position.x;
            style.top = node.Position.y;


            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));

            if (node is RootNode)
                return;

            node.onEnableChangedEvent = () =>
            {
                SetColor();
                if (!node.Enable || !node.ParentEnable)
                {
                    VisualElement top = this.Q("top");
                    top.style.backgroundColor = new Color(60, 60, 60) / 255f;
                }
            };

            node.onLinkedEvent = CheckPortState;

            if (!node.Enable || !node.ParentEnable)
            {
                VisualElement top = this.Q("top");
                top.style.backgroundColor = new Color(60, 60, 60) / 255f;
            }

            Button enableBtn = this.Q<Button>("node-enable");
            enableBtn.clicked += node.SwitchEnableState;

            RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
            RegisterCallback<MouseDownEvent>(MouseDownCallback);
            RegisterCallback<MouseOverEvent>(MouseOverCallback);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            AddPortContextualMenu(evt);
            RemoveUnusedPortContextualMenu(evt);
            if (editing)
            {
                evt.menu.AppendAction("Stop Edit Ports", (a) =>
                {
                    editing = false;
                    UpdateEditMod();
                });
            }
            else
            {
                evt.menu.AppendAction("Start Edit Ports", (a) =>
                {
                    editing = true;
                    UpdateEditMod();
                });
            }
            evt.menu.AppendAction("Reset Ports", (a) =>
            {
                node.ClearPorts();
                TreeDesignerUtility.onNodeUpdate?.Invoke(node);
            });
            node.BuildContextualMenu(evt);
            evt.StopImmediatePropagation();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Node (Set Position)");
            node.Position = new Vector2(newPos.xMin, newPos.yMin);
            EditorUtility.SetDirty(node);
        }
        public override void OnSelected()
        {
            base.OnSelected();
            onNodeSelected?.Invoke(this);
        }
        public override void OnUnselected()
        {
            base.OnUnselected();
            onNodeUnselected?.Invoke(this);
            lastTime = 0;
        }

        public virtual void SortChildren()
        {
            node.GetChildren()?.Sort(SortByPosition);
        }
        public virtual void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            switch (node.NodeState)
            {
                case BaseNode.State.Running:
                    AddToClassList("running");
                    break;
                case BaseNode.State.Failure:
                    AddToClassList("failure");
                    break;
                case BaseNode.State.Success:
                    AddToClassList("success");
                    break;
                default:
                    break;
            }
        }
        public virtual void UpdatePort()
        {
            if (node.InputPortDatas.Count == 0 && node.OutputPortDatas.Count == 0)
                ResetPorts();

            inputContainer.Clear();
            outputContainer.Clear();
            GenerateValuePorts();
        }
        
        protected void SetName()
        {
            if (node.GetType().GetCustomAttributes(typeof(NodeName), false).Length == 0)
                title = node.name;
            else
                title = (node.GetType().GetCustomAttributes(typeof(NodeName), false)[0] as NodeName).Name;

            ///ÁÙÊ±ÓÃ
            if (TreeDesignerSet.Instance.showChinese && TreeDesignerSet.Instance.NodeName(node.GetType()) != string.Empty)
            {
                title = TreeDesignerSet.Instance.NodeName(node.GetType());
            }
        }
        protected void SetColor()
        {
            if (node.GetType().GetCustomAttributes(typeof(NodeColor), true).Length > 0)
            {
                NodeColor nodeColor = null;
                foreach (var item in node.GetType().GetCustomAttributes(typeof(NodeColor), true))
                {
                    NodeColor tempNodeColor = item as NodeColor;
                    if (tempNodeColor.Name != null)
                    {
                        if (Condition(tempNodeColor.Name, tempNodeColor.Condition))
                        {
                            nodeColor = tempNodeColor;
                            break;
                        }
                    }
                    else
                    {
                        nodeColor = tempNodeColor;
                    }
                }
                if (nodeColor != null)
                {
                    VisualElement top = this.Q("top");
                    top.style.backgroundColor = nodeColor.Color / 255f;
                }
            }
        }

        protected virtual void CreateInputPorts()
        {

            var inputPorts = node.GetType().GetCustomAttributes(typeof(InputPort), true);
            if (inputPorts.Length > 0)
            {
                InputPort inputPort = inputPorts[0] as InputPort;
                if (inputPort.Name != null && !Condition(inputPort.Name, inputPort.Condition))
                    return;
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, (inputPorts[0] as InputPort).Type);
            }
            if (input != null)
            {
                input.portName = "";
                inputContainer.Add(input);
            }
        }
        protected virtual void CreateOutputPorts()
        {
 
            var outputPorts = node.GetType().GetCustomAttributes(typeof(OutputPort), true);
            if(outputPorts.Length > 0)
            {
                OutputPort outputPort = outputPorts[0] as OutputPort;
                if (outputPort.Name != null && !Condition(outputPort.Name, outputPort.Condition))
                    return;
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, (Port.Capacity)((outputPorts[0] as OutputPort).Capacity), (outputPorts[0] as OutputPort).Type);
            }

            if (output != null)
            {
                output.portName = "";
                outputContainer.Add(output);
            }
        }
        protected virtual void GenerateValuePorts()
        {
            valuePorts = new Dictionary<Port, string>();
            Type type = node.GetType();
            fieldInfos.Clear();
            inputPorts.Clear();
            outputPorts.Clear();

            foreach (var fieldInfo in type.GetFields())
            {
                if (fieldInfos.Find(i => i.Name == fieldInfo.Name) == null)
                    fieldInfos.Add(fieldInfo);
            }
            foreach (var fieldInfo in fieldInfos)
            {
                var portInfo = fieldInfo.GetCustomAttribute<PortInfo>();
                if (portInfo == null)
                    continue;
                var portIf = fieldInfo.GetCustomAttribute<PortIf>();
                if (portIf != null && !Condition(portIf.Name, portIf.Condition))
                    continue;

                string portName = portInfo.PortName;
                switch ((Direction)portInfo.Direction)
                {
                    case Direction.Input:
                        inputPorts.Add(fieldInfo, portInfo);
                        break;
                    case Direction.Output:
                        outputPorts.Add(fieldInfo, portInfo);
                        break;
                    default:
                        break;
                }
            }
            foreach (var inputPortData in node.InputPortDatas)
            {
                Port port = null;
                if(inputPortData.PortType == 0)
                {
                    InputPort inputPort = node.GetType().GetCustomAttributes(typeof(InputPort), true)[0] as InputPort;
                    input = port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, (inputPort.Type));
                    port.portName = "";

                    var connector = port.Q("connector");
                    connector.style.borderTopLeftRadius = connector.style.borderTopRightRadius = connector.style.borderBottomLeftRadius = connector.style.borderBottomRightRadius = 0;
                    var cap = connector.Q("cap");
                    cap.style.borderTopLeftRadius = cap.style.borderTopRightRadius = cap.style.borderBottomLeftRadius = cap.style.borderBottomRightRadius = 0;
                }
                else if (inputPortData.PortType == 1)
                {
                    FieldInfo fieldInfo = fieldInfos.Find(i => i.Name == inputPortData.FieldName);
                    PortInfo portInfo = inputPorts[fieldInfo];
                    port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, portInfo.PortType);
                    port.portName = portInfo.PortName;
                    valuePorts.Add(port, inputPortData.FieldName);
                }
                
                if (port != null)
                {
                    inputContainer.Add(port);
                }
            }
            foreach (var outputPortData in node.OutputPortDatas)
            {
                Port port = null;
                if(outputPortData.PortType == 0)
                {
                    OutputPort outputPort = node.GetType().GetCustomAttributes(typeof(OutputPort), true)[0] as OutputPort;
                    output = port = InstantiatePort(Orientation.Horizontal, Direction.Output, (Port.Capacity)(outputPort.Capacity), outputPort.Type);
                    port.portName = "";

                    var connector = port.Q("connector");
                    connector.style.borderTopLeftRadius = connector.style.borderTopRightRadius = connector.style.borderBottomLeftRadius = connector.style.borderBottomRightRadius = 0;
                    var cap = connector.Q("cap");
                    cap.style.borderTopLeftRadius = cap.style.borderTopRightRadius = cap.style.borderBottomLeftRadius = cap.style.borderBottomRightRadius = 0;
                }
                else if (outputPortData.PortType == 1)
                {
                    FieldInfo fieldInfo = fieldInfos.Find(i => i.Name == outputPortData.FieldName);
                    PortInfo portInfo = outputPorts[fieldInfo];
                    port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, portInfo.PortType);
                    port.portName = portInfo.PortName;
                    valuePorts.Add(port, outputPortData.FieldName);
                }

                if (port != null)
                {
                    outputContainer.Add(port);
                }
            }
        }
        protected virtual void SetupClassed()
        {
            switch (node)
            {
                case IRootNode rootNode:
                    AddToClassList("root");
                    break;
                case IActionNode actionNode:
                    AddToClassList("action");
                    break;
                case IDecoratorNode decoratorNode:
                    AddToClassList("decorator");
                    break;
                case ICompositeNode compositeNode:
                    AddToClassList("composite");
                    break;
                case ValueNode valueNode:
                    AddToClassList("value");
                    break;
                case OperateNode operateNode:
                    AddToClassList("operate");
                    break;
                case TriggerNode triggerNode:
                    AddToClassList("trigger");
                    break;
            }
        }

        protected virtual void GeometryChangedCallback(GeometryChangedEvent evt)
        {
            if (onGeometryChanged != null)
                onGeometryChanged.Invoke(this);
        }
        protected virtual void MouseDownCallback(MouseDownEvent evt)
        {
            switch (evt.button)
            {
                case 0:
                    TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                    if (ts.TotalMilliseconds - lastTime < 200)
                    {
                        onDoubleClicked?.Invoke(this);
                        lastTime = 0;
                    }
                    else
                    {
                        onMouse0Down?.Invoke(this);
                        lastTime = ts.TotalMilliseconds;
                    }
                    mouseDown = true;
                    break;
                case 2:
                    onMouse2Down?.Invoke(this);
                    break;
            }
        }
        protected virtual void MouseOverCallback(MouseOverEvent evt)
        {
            if (!mouseDown)
                return;
            mouseDown = false;
        }
        protected virtual int SortByPosition(BaseNode a, BaseNode b)
        {
            return a.Position.y < b.Position.y ? -1 : 1;
        }

        protected virtual void AddPortContextualMenu(ContextualMenuPopulateEvent evt)
        {
            Type type = node.GetType();
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            Dictionary<FieldInfo, PortInfo> inputPorts = new Dictionary<FieldInfo, PortInfo>();
            Dictionary<FieldInfo, PortInfo> outputPorts = new Dictionary<FieldInfo, PortInfo>();

            foreach (var fieldInfo in type.GetFields())
            {
                if (fieldInfos.Find(i => i.Name == fieldInfo.Name) == null)
                    fieldInfos.Add(fieldInfo);
            }
            foreach (var fieldInfo in fieldInfos)
            {
                var portInfo = fieldInfo.GetCustomAttribute<PortInfo>();
                if (portInfo == null)
                    continue;
                var portIf = fieldInfo.GetCustomAttribute<PortIf>();
                if (portIf != null && !Condition(portIf.Name, portIf.Condition))
                    continue;

                string portName = portInfo.PortName;
                switch ((Direction)portInfo.Direction)
                {
                    case Direction.Input:
                        inputPorts.Add(fieldInfo, portInfo);
                        break;
                    case Direction.Output:
                        outputPorts.Add(fieldInfo, portInfo);
                        break;
                    default:
                        break;
                }
            }
            inputPorts = inputPorts.Where(i => node.InputPortDatas.Find(j => j.FieldName == i.Key.Name) == null).ToDictionary(i => i.Key, i => i.Value);
            outputPorts = outputPorts.Where(i => node.OutputPortDatas.Find(j => j.FieldName == i.Key.Name) == null).ToDictionary(i => i.Key, i => i.Value);
            foreach (var item in inputPorts)
            {
                evt.menu.AppendAction($"Add Input Port/{item.Value.PortName}", (a) =>
                {
                    NodePortData nodePortData = node.AddInputPort(item.Key.Name);
                });
            }
            foreach (var item in outputPorts)
            {
                evt.menu.AppendAction($"Add Output Port/{item.Value.PortName}", (a) =>
                {
                    NodePortData nodePortData = node.AddOutputPort(item.Key.Name);
                });
            }
        }
        protected virtual void RemoveUnusedPortContextualMenu(ContextualMenuPopulateEvent evt)
        {
            List<NodePortData> unusedPortDatas = new List<NodePortData>();
            foreach (var item in valuePorts)
            {
                if (!item.Key.connected)
                {
                    if (item.Key.direction == Direction.Input)
                    {
                        NodePortData inputPortData = node.InputPortDatas.Find(i => i.FieldName == item.Value);
                        FieldInfo fieldInfo = fieldInfos.Find(i => i.Name == inputPortData.FieldName);
                        PortInfo portInfo = inputPorts[fieldInfo];
                        if(!PortRequired(portInfo,inputPortData))
                            unusedPortDatas.Add(inputPortData);
                    }
                    else if (item.Key.direction == Direction.Output)
                        unusedPortDatas.Add(node.OutputPortDatas.Find(i => i.FieldName == item.Value));
                }
            }
            if (unusedPortDatas.Count == 0) return;

            evt.menu.AppendAction("Remove UnusedPorts", (a) =>
            {
                node.RemovePorts(unusedPortDatas);
            });
        }

        protected virtual void UpdateEditMod()
        {
            foreach (var inputPortData in node.InputPortDatas)
            {
                Port port = null;
                if(inputPortData.PortType == 0)
                {
                    port = input;
                }
                else if (inputPortData.PortType == 1)
                {
                    foreach (var valuePort in valuePorts)
                    {
                        if(valuePort.Value == inputPortData.FieldName)
                        {
                            port = valuePort.Key;
                            break;
                        }
                    }
                }

                if (editing)
                {
                    DrawEditInputPort(port, inputPortData);
                }
                else
                {
                    Button upButton = port.Q<Button>("upBtn");
                    port.Remove(upButton);
                    Button downButton = port.Q<Button>("downBtn");
                    port.Remove(downButton);
                }
            }
            foreach (var outputPortData in node.OutputPortDatas)
            {
                Port port = null;
                if (outputPortData.PortType == 0)
                {
                    port = output;
                }
                else if (outputPortData.PortType == 1)
                {
                    foreach (var valuePort in valuePorts)
                    {
                        if (valuePort.Value == outputPortData.FieldName)
                        {
                            port = valuePort.Key;
                            break;
                        }
                    }
                }

                if (editing)
                {
                    DrawEditOutputPort(port, outputPortData);
                }
                else
                {
                    Button upButton = port.Q<Button>("upBtn");
                    port.Remove(upButton);
                    Button downButton = port.Q<Button>("downBtn");
                    port.Remove(downButton);
                }
            }
        }
        protected virtual void ResetPorts()
        {
            node.ClearPorts();
            List<Type> types = new List<Type>();
            Type targetType = node.GetType();
            while (targetType != null)
            {
                types.Add(targetType);
                targetType = targetType.BaseType;
            }
            types.Reverse();

            List<FieldInfo> fields = new List<FieldInfo>();
            foreach (var type in types)
            {
                foreach (var fieldInfo in type.GetFields())
                {
                    if (fields.Find(i => i.Name == fieldInfo.Name) == null)
                        fields.Add(fieldInfo);
                }
            }

            Dictionary<FieldInfo, PortInfo> inputPorts = new Dictionary<FieldInfo, PortInfo>();
            Dictionary<FieldInfo, PortInfo> outputPorts = new Dictionary<FieldInfo, PortInfo>();

            foreach (var item in fields)
            {
                var portInfo = item.GetCustomAttribute<PortInfo>();
                if (portInfo == null)
                    continue;
                var portIf = item.GetCustomAttribute<PortIf>();
                if (portIf != null && !Condition(portIf.Name, portIf.Condition))
                    continue;

                string portName = portInfo.PortName;
                switch ((Direction)portInfo.Direction)
                {
                    case Direction.Input:
                        inputPorts.Add(item, portInfo);
                        break;
                    case Direction.Output:
                        outputPorts.Add(item, portInfo);
                        break;
                    default:
                        break;
                }
            }
            int index = 0;
            var inputPortAttributes = node.GetType().GetCustomAttributes(typeof(InputPort), true);
            if (inputPortAttributes.Length > 0)
            {
                InputPort inputPort = inputPortAttributes[0] as InputPort;
                if (inputPort.Name == null || Condition(inputPort.Name, inputPort.Condition))
                {
                    node.InputPortDatas.Add(new NodePortData("m_InputPort", index,0));
                    index++;
                }
            }
            foreach (var inputPort in inputPorts)
            {
                node.InputPortDatas.Add(new NodePortData(inputPort.Key.Name, index,1));
                index++;
            }
            index = 0;
            var outputPortAttributs = node.GetType().GetCustomAttributes(typeof(OutputPort), true);
            if (outputPortAttributs.Length > 0)
            {
                OutputPort outputPort = outputPortAttributs[0] as OutputPort;
                if (outputPort.Name == null || Condition(outputPort.Name, outputPort.Condition))
                {
                    node.OutputPortDatas.Add(new NodePortData("m_OutputPort", index,0));
                    index++;
                }
            }
            foreach (var outputPort in outputPorts)
            {
                node.OutputPortDatas.Add(new NodePortData(outputPort.Key.Name, index,1));
                index++;
            }
        }

        

        void CheckPortState()
        {
            foreach (var inputPortData in node.InputPortDatas)
            {
                Port port = null;
                foreach (var portNamePair in valuePorts)
                {
                    if (portNamePair.Key.direction == Direction.Input && portNamePair.Value == inputPortData.FieldName)
                        port = portNamePair.Key;
                }
                if (port == null)
                    continue;
                if (inputPortData.PortType == 1)
                {
                    FieldInfo fieldInfo = fieldInfos.Find(i => i.Name == inputPortData.FieldName);
                    PortInfo portInfo = inputPorts[fieldInfo];
                    
                    if (PortRequired(portInfo, inputPortData) && !node.IsConnected(inputPortData.FieldName))
                    {
                        Color errorBgColor = new Color(1, 0.325f, 0.325f, 1);
                        port.style.backgroundColor = errorBgColor;

                        Color errorNameColor = Color.black;
                        Label nameLabel = port.Q<Label>("type");
                        nameLabel.style.color = errorNameColor;
                    }
                    else
                    {
                        Color defaultBgColor = new Color(0, 0, 0, 0);
                        port.style.backgroundColor = defaultBgColor;

                        Color defaultNameColor = new Color(193, 193, 193, 255) / 255;
                        Label nameLabel = port.Q<Label>("type");
                        nameLabel.style.color = defaultNameColor;
                    }
                }
            }
        }
        void DrawEditInputPort(Port port, NodePortData nodePortData)
        {
            Button upButton = PortButton("upBtn", "¡ü", () =>
            {
                if (node.MovePort(node.InputPortDatas, nodePortData, -1))
                {
                    int originalIndex = inputContainer.IndexOf(port);
                    inputContainer.Remove(port);
                    inputContainer.Insert(originalIndex - 1, port);
                }
            });
            upButton.style.marginRight = 1;

            Button downButton = PortButton("downBtn", "¡ý", () =>
            {
                if (node.MovePort(node.InputPortDatas, nodePortData, 1))
                {
                    int originalIndex = inputContainer.IndexOf(port);
                    inputContainer.Remove(port);
                    inputContainer.Insert(originalIndex + 1, port);
                }
            });
            downButton.style.marginLeft = 1;

            port.Insert(1, upButton);
            port.Insert(2, downButton);
        }
        void DrawEditOutputPort(Port port, NodePortData nodePortData)
        {
            Button upButton = PortButton("upBtn", "¡ü", () =>
            {
                if (node.MovePort(node.OutputPortDatas, nodePortData, -1))
                {
                    int originalIndex = outputContainer.IndexOf(port);
                    outputContainer.Remove(port);
                    outputContainer.Insert(originalIndex - 1, port);
                }
            });
            upButton.style.marginRight = 1;

            Button downButton = PortButton("downBtn", "¡ý", () =>
            {
                if (node.MovePort(node.OutputPortDatas, nodePortData, 1))
                {
                    int originalIndex = outputContainer.IndexOf(port);
                    outputContainer.Remove(port);
                    outputContainer.Insert(originalIndex + 1, port);
                }
            });
            downButton.style.marginLeft = 1;

            port.Insert(1, downButton);
            port.Insert(2, upButton);
        }
        Button PortButton(string buttonName,string buttonText,Action onClick)
        {
            Button button = new Button();
            button.name = buttonName;
            button.text = buttonText;
            button.style.borderTopWidth = button.style.borderBottomWidth = button.style.borderLeftWidth = button.style.borderRightWidth = 1;
            button.style.borderTopLeftRadius = button.style.borderTopRightRadius = button.style.borderBottomLeftRadius = button.style.borderBottomRightRadius = 0;
            button.style.borderTopColor = button.style.borderBottomColor = button.style.borderLeftColor = button.style.borderRightColor = new Color(196, 196, 196);
            button.style.marginTop = button.style.marginBottom = button.style.marginLeft = button.style.marginRight = 0;
            button.style.paddingTop = button.style.paddingBottom = button.style.paddingLeft = button.style.paddingRight = 0;
            button.style.height = 16;
            button.style.width = 16;
            button.clicked += onClick;
            return button;
        }

        bool PortRequired(PortInfo portInfo, NodePortData nodePortData)
        {
            if (portInfo.Required)
                return true;
            else if (!string.IsNullOrEmpty(portInfo.RequiredReference) && Condition(portInfo.RequiredReference, portInfo.RequiredCondition))
                return true;
            return false;
        }
        bool Condition(string name,object condition)
        {
            bool pass = false;

            Type t = node.GetType();
            FieldInfo f = null;
            PropertyInfo p = null;
            MethodInfo m = null;
            object targetValue = null;

            f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            m = t.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (f != null)
                targetValue = f.GetValue(node);
            else if (p != null)
                targetValue = p.GetValue(node);
            else if (m != null)
                targetValue = m.Invoke(node, null);

            pass = condition.Equals(targetValue);
            return pass;
        }
    }
}
#endif