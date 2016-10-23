using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Composite
    {
        public Selector()
            : base(null)
        { }

        public Selector(params Node[] nodes)
            : base(nodes)
        { }

        public override bool Run()
        {
            for (int i = 0; i < ChileNodeList.Count; i++)
            {
                if (ChileNodeList[i].Run())
                    return true;
            }
            return false;
        }
    }
}
