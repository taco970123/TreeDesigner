using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("Random")]
    [NodePath("Base/Decorator/RandomNode")]
    public class RandomNode : DecoratorNode
    {
        [PortInfo("Probability(0-100)", 0, typeof(float))]
        public float probability;
        [ReadOnly]
        public float result;

        public bool alwaysSuccess;

        protected override void DoAction()
        {
            result = Random.Range(0f, 100f);
        }
        protected override State OnUpdate()
        {
            if(!child || !child.Enable || alwaysSuccess)
                return State.Success;
            else if (result <= probability)
                return child.UpdateState();
            else
                return State.Failure;
        }
    }
}
