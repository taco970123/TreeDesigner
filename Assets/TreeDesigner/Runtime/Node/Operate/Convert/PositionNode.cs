using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("Position")]
    [NodePath("Base/Operate/Convert/PositionNode")]
    public class PositionNode : OperateNode
    {
        [PortInfo("Transform", 0, typeof(Transform)), ReadOnly]
        public Transform transform;
        [PortInfo("Position", 1, typeof(Vector3)), ReadOnly]
        public Vector3 vector3Value;

        protected override void Operate()
        {
            vector3Value = transform.position;
        }
    }
}