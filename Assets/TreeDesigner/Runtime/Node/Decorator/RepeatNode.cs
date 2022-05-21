namespace TreeDesigner.Runtime
{
    [NodeName("Repeat")]
    [NodePath("Base/Decorator/RepeatNode")]
    public class RepeatNode : DecoratorNode
    {
        [PortInfo("Count", 0, typeof(int))]
        public int count;

        int currentIndex;

        protected override void DoAction()
        {
            currentIndex = 0;
        }

        protected override State OnUpdate()
        {
            while (currentIndex < count && child.Enable)
            {
                switch (child.UpdateState())
                {
                    case State.Running:
                        return State.Running;
                    case State.Failure:
                        currentIndex++;
                        break;
                    case State.Success:
                        currentIndex++;
                        break;
                }
            }
            return State.Success;
        }
    }
}