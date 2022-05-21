namespace TreeDesigner.Runtime 
{
    [System.Serializable]
    public class NodeLinkData
    {
        public BaseNode sourceNode;
        public string outputValueName;
        public string inputValueName;

        public NodeLinkData(BaseNode sourceNode, string outputValueName, string inputValueName)
        {
            this.sourceNode = sourceNode;
            this.outputValueName = outputValueName;
            this.inputValueName = inputValueName;
        }
    }
}