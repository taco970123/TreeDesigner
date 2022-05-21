using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("Wait")]
    [NodePath("Base/Decorator/WaitNode")]
    public class WaitNode : DecoratorNode
    {
        [PortInfo("Time", 0, typeof(float))]
        public float duration = 1;

        float startTime;

        protected override void DoAction()
        {
            startTime = Time.time;
        }
        protected override State OnUpdate()
        {
            if (Time.time - startTime > duration)
            {
                return child.Enable ? child.UpdateState() : State.Success;
            }
            else
                return State.Running;
        }
    }
}