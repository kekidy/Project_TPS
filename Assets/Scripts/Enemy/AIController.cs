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
        Sequence sequence1 = new Sequence();

        root.SetChild = select1;
            select1.AddChile(inverter1);
                inverter1.SetChild = new Action(IsPlayerDetect);
            select1.AddChile(sequence1);
                sequence1.AddChile(new Action(LookAtTarget));
                sequence1.AddChile(new Action(AttackToTarget));

        this.UpdateAsObservable()
            .Subscribe(_ => root.Run());
    }

    void Start()
    { }


    private bool IsPlayerDetect()
    {
        return m_runnerBotCtrl.IsPlayerDetect;
    }

    private bool LookAtTarget()
    {
        m_runnerBotCtrl.transform.LookAt(m_playerCtrl.transform);
        Debug.Log("Look At");
        return true;
    }

    private bool AttackToTarget()
    {
        Debug.Log("Attack To Target");
        return true;
    }
}
