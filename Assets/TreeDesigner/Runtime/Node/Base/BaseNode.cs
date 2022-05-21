using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using TreeDesigner.Editor;
#endif

namespace TreeDesigner.Runtime
{
    public abstract class BaseNode : ScriptableObject
    {
        public enum State
        {
            Default = 0,
            Success = 1,
            Failure = 2,
            Running = 3,
        }

        [SerializeField, HideInInspector]
        bool parentEnable;
        [SerializeField, HideInInspector]
        bool enable;
        [SerializeField, HideInInspector]
        protected List<NodeLinkData> linkDatas = new List<NodeLinkData>();
        [SerializeField, HideInInspector]
        protected List<NodePortData> inputPortDatas = new List<NodePortData>();
        [SerializeField, HideInInspector]
        protected List<NodePortData> outputPortDatas = new List<NodePortData>();

        [ShowIf("HasValueSource", true)]
        public bool resetValueSource;

        protected bool started;
        protected State nodeState;


        protected BaseTree owner;

        /// <summary>
        /// 来源FieldInfo,目标FieldInfo
        /// </summary>
        protected List<(FieldInfo, FieldInfo)> linkValues;

        public bool ParentEnable { get => parentEnable; set => parentEnable = value; }
        public bool Enable { get => enable; set => enable = value; }
        public BaseTree Owner { get => owner; set => owner = value; }
        public List<NodeLinkData> LinkDatas => linkDatas;
        public List<NodePortData> InputPortDatas { get => inputPortDatas; set => inputPortDatas = value; }
        public List<NodePortData> OutputPortDatas { get => outputPortDatas; set => outputPortDatas = value; }

        public bool Started => started;
        public virtual bool CanAddChild => true;
        public State NodeState => nodeState;

        public void ResetState()
        {
            started = false;
            nodeState = State.Default;
            OnReset();
        }
        public virtual State UpdateState()
        {
            try
            {
                if (!started)
                {
                    OnStart();
                    started = true;
                }

                nodeState = OnUpdate();

                if (nodeState == State.Failure || nodeState == State.Success)
                {
                    OnStop();
                    started = false;
                }
            }
            catch (Exception ex)
            {
                nodeState = State.Failure;
                Debug.LogError($"{owner.name}:{owner.treeName}");
                Debug.LogException(ex);
            }
            return nodeState;
        }

        public abstract BaseNode Clone();
        public abstract List<BaseNode> GetChildren();
        public void UpdateSharedValue(Dictionary<ExposedProperty, ExposedProperty> exposedPropertyPairs)
        {
            Type thisType = GetType();
            foreach (var ttfi in thisType.GetFields())
            {
                if (ttfi.GetValue(this) is ExposedProperty exposedProperty)
                {
                    if (exposedProperty.Global)
                    {
                        ttfi.SetValue(this, ExposedProperty.GetRuntimeGlobalProperty(exposedProperty));
                    }
                    else
                    {
                        if (exposedPropertyPairs.TryGetValue(exposedProperty, out ExposedProperty cloneExposedProperty))
                        {
                            ttfi.SetValue(this, cloneExposedProperty);
                        }
                    }
                }
            }
        }
        public void UpdateLinkValue()
        {
            linkValues = new List<(FieldInfo, FieldInfo)>();
            foreach (var linkData in linkDatas)
            {
                linkValues.Add((linkData.sourceNode.OutFieldInfo(linkData.outputValueName), InFieldInfo(linkData.inputValueName)));
            }
        }

        protected virtual FieldInfo OutFieldInfo(string name)
        {
            return GetType().GetField(name);
        }
        protected virtual FieldInfo InFieldInfo(string name)
        {
            return GetType().GetField(name);
        }
        protected virtual object OutValue(FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(this);
        }
        protected virtual void InValue(FieldInfo fieldInfo,object value)
        {
            fieldInfo.SetValue(this, value);
        }
        protected virtual void GetValue()
        {
            foreach (var linkData in linkDatas)
            {
                if (resetValueSource)
                    linkData.sourceNode.ResetState();
                if (linkData.sourceNode.enable)
                    linkData.sourceNode.GetValue();
            }
            for (int i = 0; i < linkDatas.Count; i++)
            {
                InValue(linkValues[i].Item2, linkDatas[i].sourceNode.OutValue(linkValues[i].Item1));
            }
            OnGetValue();
        }
        protected abstract void OnReset();
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
        protected virtual void OnGetValue() { }
        public abstract void AddChild(BaseNode baseNode);
        public abstract void RemoveChild(BaseNode baseNode);

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        string guid;
        [SerializeField, HideInInspector]
        Vector2 position;
        [HideInInspector]
        string description;


        public virtual bool IsSelectable => true;
        public virtual bool IsCopiable => true;
        public virtual bool IsDeletable => true;
        public string GUID { get => guid; set => guid = value; }
        public Vector2 Position { get => position; set => position = value; }

        public Action onEnableChangedEvent;
        public Action onLinkedEvent;

        public void Link(BaseNode sourceNode, string outputValueName, string inputValueName)
        {
            linkDatas.Add(new NodeLinkData(sourceNode, outputValueName, inputValueName));
            onLinkedEvent?.Invoke();
        }
        public void UnLink(BaseNode sourceNode, string outputValueName, string inputValueName)
        {
            NodeLinkData buffNodeLinkData = linkDatas.Find((i) => i.sourceNode == sourceNode && i.outputValueName == outputValueName && i.inputValueName == inputValueName);
            if (buffNodeLinkData != null)
            {
                linkDatas.Remove(buffNodeLinkData);
            }
            onLinkedEvent?.Invoke();
        }
        public virtual void SwitchEnableState()
        {
            enable = !enable;
            SetParentEnableState(parentEnable);
            onEnableChangedEvent?.Invoke();
        }
        public virtual void SetParentEnableState(bool parentEnable)
        {
            this.parentEnable = parentEnable;
            onEnableChangedEvent?.Invoke();
        }
        public virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Edit Script", (a) =>
            {
                MonoScript[] array = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != null && array[i].GetClass() != null && array[i].GetClass().Equals(GetType()))
                    {
                        AssetDatabase.OpenAsset(array[i].GetInstanceID(), 0, 0);
                        break;
                    }
                }
            });
            evt.menu.AppendAction("Locate Script", (a) =>
            {
                MonoScript[] array = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != null && array[i].GetClass() != null && array[i].GetClass().Equals(GetType()))
                    {
                        Selection.activeObject = array[i];
                        break;
                    }
                }
            });
        }

        public bool HasValueSource()
        {
            return linkDatas.Count > 0;
        }

        public bool IsConnected(string inputName)
        {
            return linkDatas.Find(i=>i.inputValueName == inputName) != null;
        }
        public void ClearPorts()
        {
            Undo.RecordObject(this, "Node (Clear Ports)");
            inputPortDatas.Clear();
            outputPortDatas.Clear();
            EditorUtility.SetDirty(this);
        }
        public bool MovePort(List<NodePortData> list, NodePortData element, int index)
        {
            Undo.RecordObject(this, "Node (Move Port)");
            int oringinalIndex = list.IndexOf(element);
            int targetIndex = oringinalIndex + index;
            targetIndex = Mathf.Clamp(targetIndex, 0, list.Count - 1);
            NodePortData temp = list[targetIndex];
            list[targetIndex] = element;
            list[oringinalIndex] = temp;
            return oringinalIndex != targetIndex;
        }
        public void RemovePort(NodePortData port)
        {
            if(inputPortDatas.Contains(port))
                inputPortDatas.Remove(port);
            else if (outputPortDatas.Contains(port))
                outputPortDatas.Remove(port);
            EditorUtility.SetDirty(this);
            TreeDesignerUtility.onNodeUpdate?.Invoke(this);
        }
        public void RemovePorts(List<NodePortData> ports)
        {
            Undo.RecordObject(this, "Node (Remove UnusedPorts)");
            ports.ForEach(i =>
            {
                if (inputPortDatas.Contains(i))
                    inputPortDatas.Remove(i);
                else if (outputPortDatas.Contains(i))
                    outputPortDatas.Remove(i);
            });
            EditorUtility.SetDirty(this);
            TreeDesignerUtility.onNodeUpdate?.Invoke(this);
        }
        public NodePortData AddInputPort(string fieldName)
        {
            Undo.RecordObject(this, "Node (Add Port)");
            NodePortData nodePortData = new NodePortData(fieldName, inputPortDatas.Count, 1);
            inputPortDatas.Add(nodePortData);
            EditorUtility.SetDirty(this);
            TreeDesignerUtility.onNodeUpdate?.Invoke(this);
            return nodePortData;
        }
        public NodePortData AddOutputPort(string fieldName)
        {
            Undo.RecordObject(this, "Node (Add Port)");
            NodePortData nodePortData = new NodePortData(fieldName, outputPortDatas.Count, 1);
            outputPortDatas.Add(nodePortData);
            EditorUtility.SetDirty(this);
            TreeDesignerUtility.onNodeUpdate?.Invoke(this);
            return nodePortData;
        }
#endif
    }
}

