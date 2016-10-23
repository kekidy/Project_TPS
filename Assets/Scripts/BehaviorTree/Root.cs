using UnityEngine;
using System.Collections;
using System;

namespace BehaviorTree
{
    public class Root : Decorator
    {
        public Root()
            : base(null)
        { }

        public Root(Node node)
            : base(node)
        { }

        public override bool Run()
        {
            return m_chileNode.Run();
        }
    }
}