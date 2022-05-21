namespace TreeDesigner.Runtime
{
    [NodeName("OnceSequence")]
    [NodePath("Base/Composite/OnceSequenceNode")]
    public class OnceSequenceNode : CompositeNode
    {
        [LabelAs("ChangedOutSide")]
        public bool changedOutSide;
        [ShowIf("changedOutSide", true), LabelAs("Index")]
        public ExposedIntProperty exposedInt;

        int currentIndex;

        protected override void OnReset() { }
        protected override State OnUpdate()
        {
            if (changedOutSide)
            {
                while (exposedInt.Value < children.Count)
                {
                    var child = children[exposedInt.Value];
                    if (child.Enable == false)
                    {
                        exposedInt.Value = (exposedInt.Value + 1) % children.Count;
                        continue;
                    }
                    switch (child.UpdateState())
                    {
                        case State.Running:
                            return State.Running;
                        case State.Failure:
                            exposedInt.Value = (exposedInt.Value + 1) % children.Count;
                            return State.Failure;
                        case State.Success:
                            exposedInt.Value = (exposedInt.Value + 1) % children.Count;
                            return State.Success;
                    }
                }
                return State.Success;
            }
            else
            {
                while (currentIndex < children.Count)
                {
                    var child = children[currentIndex];
                    if (child.Enable == false)
                    {
                        currentIndex = (currentIndex + 1) % children.Count;
                        continue;
                    }
                    switch (child.UpdateState())
                    {
                        case State.Running:
                            return State.Running;
                        case State.Failure:
                            currentIndex = (currentIndex + 1) % children.Count;
                            return State.Failure;
                        case State.Success:
                            currentIndex = (currentIndex + 1) % children.Count;
                            return State.Success;
                    }
                }
                return State.Success;
            }
        }
    }
}