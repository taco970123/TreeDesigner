using UnityEngine;

namespace TreeDesigner.Runtime
{
    [System.Serializable]
    public class NodeValueData
    {
        [SerializeField]
        string _name;
        [SerializeField]
        object _value;

        public NodeValueData(string name)
        {
            _name = name;
            _value = new object();
        }

        public string Name => _name;
        public object Value { get => _value; set => _value = value; }
    }
}
