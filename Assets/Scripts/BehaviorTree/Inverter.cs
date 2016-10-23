using UnityEngine;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class Inverter : Decorator
    {
        public Inverter()
            : base(null)
        { }

        public Inverter(Node node)
            : base(node)
        { }

        public override bool Run()
        {
            return !m_chileNode.Run();
        }
    }
}