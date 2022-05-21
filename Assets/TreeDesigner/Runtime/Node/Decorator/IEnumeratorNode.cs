using System.Collections;

namespace TreeDesigner.Runtime
{
    [NodeName("IEnumerator")]
    [NodePath("Base/Decorator/IEnumeratorNode")]
    public class IEnumeratorNode : DecoratorNode
    {
        public enum ResultType
        {
            Always = 0,
            Once = 1,
            Never = 2,
        }

        [PortInfo("List", 0, typeof(IList)), ReadOnly]
        public object valueList;
        [PortInfo("Value", 1, typeof(object)), ReadOnly]
        public object value;

        public ResultType resultType;

        bool success;
        int currentIndex;
        State lastState;
        IList targetList;

        protected override void DoAction()
        {
            success = false;
            currentIndex = 0;
            lastState = State.Success;
            targetList = (IList)valueList;
        }
        protected override State OnUpdate()
        {
            while (currentIndex < targetList.Count && child.Enable)
            {
                if(lastState != State.Running)
                {
                    value = targetList[currentIndex];
                }
                lastState = child.UpdateState();
                switch (lastState)
                {
                    case State.Running:
                        return State.Running;
                    case State.Failure:
                        currentIndex++;
                        break;
                    case State.Success:
                        success = true;
                        currentIndex++;
                        break;
                }
            }
            switch (resultType)
            {
                case ResultType.Always:
                    return State.Success;
                case ResultType.Once:
                    return success ? State.Success : State.Failure;
                case ResultType.Never:
                    return State.Failure;
                default:
                    return State.Default;
            }
        }
    }
}