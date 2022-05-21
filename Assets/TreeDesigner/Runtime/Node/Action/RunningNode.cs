namespace TreeDesigner.Runtime
{
    [NodeName("Running")]
    [NodePath("Base/Action/RunningNode")]
    public class RunningNode : ActionNode
    {
        protected override State OnUpdate()
        {
            return State.Running;
        }
        protected override void DoAction() { }
    }
}
