using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using BehaviorTree;
using EasyEditor;

public class RunnerBotCtrl : MonoBehaviour {
    [Inspector(group = "IK Info")]
    [SerializeField] private Transform m_leftHandTarge = null;
    private Animator m_myAnim           = null;   
    private float    m_ikLeftHandWeight = 1f;

    public bool IsPlayerDetect { get; private set; }

	void Awake () {
        m_myAnim = GetComponent<Animator>();
	}

    void Start()
    {
    }
	
    void OnAnimatorIK(int layer)
    {
        m_ikLeftHandWeight = Mathf.MoveTowards(m_ikLeftHandWeight, 1f, Time.smoothDeltaTime * 2f);
        m_myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_ikLeftHandWeight);
        m_myAnim.SetIKPosition(AvatarIKGoal.LeftHand, m_leftHandTarge.position);
    }

    [Inspector(group = "AI Test")]
    private void InverseIsPlayerDetect()
    {
        IsPlayerDetect = !IsPlayerDetect;
    }
}
