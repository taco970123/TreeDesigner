using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("RandomSelector")]
    [NodePath("Base/Composite/RandomSelectorNode")]
    public class RandomSelectorNode : CompositeNode
    {
        public List<float> randoms = new List<float>();


        [LabelAs("Result"), ReadOnly]
        public float random;
        BaseNode child;
        Dictionary<int, float> randomRange;

        protected override void OnReset()
        {
            random = 0;
            child = null;
            randomRange = new Dictionary<int, float>();

            float randomStep = 0;
            for (int i = 0; i < children.Count; i++)
            {
                randomRange.Add(i, randoms[i] + randomStep);
                randomStep += randoms[i];
            }
            random = Random.Range(0f, 100f);

            foreach (var item in randomRange)
            {
                if (random <= item.Value)
                {
                    child = children[item.Key];
                    break;
                }
            }
        }
        protected override State OnUpdate()
        {
            if (child != null && child.Enable)
                return child.UpdateState();
            return State.Success;
        }

#if UNITY_EDITOR
        public override bool CanAddChild => children.Count < randoms.Count;
#endif
    }
}