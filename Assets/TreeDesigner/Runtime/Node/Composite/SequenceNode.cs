namespace TreeDesigner.Runtime
{
    [NodeName("Sequence")]
    [NodePath("Base/Composite/SequenceNode")]
    public class SequenceNode : CompositeNode
    {
        [LabelAs("ChangedOutSide")]
        public bool changedOutSide;
        [ShowIf("changedOutSide", true), LabelAs("Index")]
        public ExposedIntProperty exposedInt;

        int currentIndex;

        protected override void OnReset()
        {
            currentIndex = 0;
            if(exposedInt)
                exposedInt.Value = 0;
        }
        protected override State OnUpdate()
        {
            if (changedOutSide)
            {
                while (exposedInt.Value < children.Count)
                {
                    var child = children[exposedInt.Value];
                    if (!child.Enable)
                    {
                        exposedInt.Value++;
                        continue;
                    }
                    switch (child.UpdateState())
                    {
                        case State.Failure:
                            return State.Failure;
                        case State.Success:
                            exposedInt.Value++;
                            break;
                        case State.Running:
                            return State.Running;
                    }
                }
                return State.Success;
            }
            else
            {
                while (currentIndex < children.Count)
                {
                    var child = children[currentIndex];
                    if (!child.Enable)
                    {
                        currentIndex++;
                        continue;
                    }

                    switch (child.UpdateState())
                    {
                        case State.Failure:
                            return State.Failure;
                        case State.Success:
                            currentIndex++;
                            break;
                        case State.Running:
                            return State.Running;
                    }
                }
                return State.Success;
            }
        }

        protected override void OnStop()
        {
            currentIndex = 0;
            if (exposedInt)
                exposedInt.Value = 0;
        }
    }
}