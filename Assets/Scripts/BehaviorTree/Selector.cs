using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Composite
    {
        public Selector(params Node[] nodes)
            : base(nodes)
        {

        }

        public override bool Run()
        {
            List<Node> nodeList = ChileNodeList;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].Run())
                    return true;
            }
            return false;
        }
    }
}
