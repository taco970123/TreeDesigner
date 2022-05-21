namespace TreeDesigner.Runtime
{
    public interface IBaseNode { }
    public interface IRootNode { }
    public interface IActionNode { }
    public interface ICompositeNode { }
    public interface IDecoratorNode { }

    public interface IOutsideValueNode
    {
        public object OutsideValue { get; set; }
    }
}