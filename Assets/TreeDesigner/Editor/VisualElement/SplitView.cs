#if UNITY_EDITOR
using UnityEngine.UIElements;

#if UNITY_2020_3_OR_NEWER
namespace TreeDesigner.Editor
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
        public SplitView() { }
    }
}
#endif
#endif