using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Composite
    {
        public Sequence()
            : base(null)
        { }

        public Sequence(params Node[] nodes)
            : base(nodes)
        { }

        public override bool Run()
        {
            for (int i = 0; i < ChileNodeList.Count; i++)
            {
                if (!ChileNodeList[i].Run())
                    return false;
            }
            return true;
        }
    }
}
