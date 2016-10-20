using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Composite
    {
        public Sequence(params Node[] nodes)
            : base(nodes)
        {

        }

        public override bool Run()
        {
            List<Node> nodeList = ChileNodeList;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].Run())
                    return false;
            }
            return true;
        }
    }
}
