
namespace TreeDesigner.Runtime
{
    public abstract class JudgeNode : DecoratorNode
    {
        protected bool isRight;
        public bool alwaysRight;

        protected sealed override State OnUpdate()
        {
            if (isRight || alwaysRight)
                return (child && child.Enable) ? child.UpdateState() : State.Success;
            else
                return State.Failure;
        }
        protected sealed override void DoAction()
        {
            Judge();
        }
    
        protected abstract void Judge();
    }
}