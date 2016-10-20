using UnityEngine;
using System.Collections;

namespace BehaviorTree
{
    public abstract class Decorator : Node
    {
        protected Node m_chileNode;

        public Decorator(Node node)
        {
            m_chileNode = node;
        }

        public Node SetChild { set { m_chileNode = value; } }
    }
}