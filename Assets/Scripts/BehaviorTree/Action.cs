using UnityEngine;
using System.Collections;
using System;

namespace BehaviorTree
{
    public class Action : Node
    {
        Func<bool> m_actionFunc;

        public Action(Func<bool> actionFunc)
        {
            m_actionFunc = actionFunc;
        }

        public override bool Run()
        {
            return m_actionFunc();
        }
    }
}
