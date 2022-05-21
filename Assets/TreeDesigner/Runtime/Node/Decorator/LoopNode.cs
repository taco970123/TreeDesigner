namespace TreeDesigner.Runtime
{
    [NodeName("Loop")]
    [NodePath("Base/Decorator/LoopNode")]
    public class LoopNode : DecoratorNode
    {
        protected override void DoAction() { }
        protected override State OnUpdate()
        {
            if (child.Enable)
            {
                child.UpdateState();
                return State.Running;
            }
            else
                return State.Success;
        }
    }
}
