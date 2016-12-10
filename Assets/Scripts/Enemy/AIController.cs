using UnityEngine;
using BehaviorTree;
using UniRx;
using UniRx.Triggers;
using EasyEditor;
using System.Collections;

[RequireComponent(typeof(RunnerBotCtrl))]
public class AIController : MonoBehaviour {
    private RunnerBotCtrl m_runnerBotCtrl = null;
    private PlayerCtrl    m_playerCtrl    = null;

    void Awake()
    {
        m_runnerBotCtrl = GetComponent<RunnerBotCtrl>();
        m_playerCtrl = GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();

        Root root = new Root();
        Selector select1   = new Selector();
        Inverter inverter1 = new Inverter();
        Selector select2   = new Selector();
        Sequence sequence1 = new Sequence();

        root.SetChild = select1;
            select1.AddChile(new Action(IsDead));
            select1.AddChile(inverter1);
                inverter1.SetChild = new Action(() => m_runnerBotCtrl.IsPlayerDetect);
            select1.AddChile(select2);
                select2.AddChile(new Action(() => m_runnerBotCtrl.IsAttacking));
                select2.AddChile(new Action(AttackToTarget));

        this.UpdateAsObservable()
            .Subscribe(_ => root.Run());
    }

    private bool IsDead()
    {
        if (m_runnerBotCtrl.IsDead)
        {
            m_runnerBotCtrl.Anim.SetTrigger("isDead");

            Destroy(this);
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<ObservableUpdateTrigger>());
            Destroy(GetComponent<RunnerBotCtrl>());
            
            return true;
        }
        else
            return false;
    }

    private bool LookAtTarget()
    {
        m_runnerBotCtrl.transform.LookAt(m_playerCtrl.transform);
        return true;
    }

    private bool AttackToTarget()
    {
        m_runnerBotCtrl.StartAttackToTarget();
        return true;
    }
}
