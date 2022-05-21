using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [Serializable]
    public class NodePortData
    {
        [SerializeField]
        string m_FieldName;
        [SerializeField]
        int m_PortType;
        [SerializeField]
        int m_PortIndex;

        public string FieldName => m_FieldName;
        public int PortType => m_PortType;
        public int PortIndex { get => m_PortIndex; set => m_PortIndex = value; }

        public NodePortData(string fieldName, int portIndex,int portType)
        {
            m_FieldName = fieldName;
            m_PortType = portType;
            m_PortIndex = portIndex;
        }
    }
}
