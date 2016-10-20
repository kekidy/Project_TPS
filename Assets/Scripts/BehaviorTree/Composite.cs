using System.Collections;
using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class Composite : Node
    {
        private List<Node> m_chileNodeList = new List<Node>();

        public List<Node> ChileNodeList { get { return m_chileNodeList; } }

        public Composite(params Node[] nodes)
        {
            m_chileNodeList.AddRange(nodes);
        }

        public void AddChile(Node node)
        {
            m_chileNodeList.Add(node);
        }
    }
}
