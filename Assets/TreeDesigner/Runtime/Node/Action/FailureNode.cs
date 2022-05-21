namespace TreeDesigner.Runtime
{
    [NodeName("Failure")]
    [NodePath("Base/Action/FailureNode")]
    public class FailureNode : ActionNode
    {
        protected override State OnUpdate()
        {
            return State.Failure;
        }
        protected override void DoAction() { }
    }
}
