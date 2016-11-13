using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using BehaviorTree;
using EasyEditor;

public class RunnerBotCtrl : MonoBehaviour {
    [Inspector(group = "Runner Bot Status")]
    [SerializeField] private float m_hp = 0f;

    [Inspector(group = "IK Info")]
    [SerializeField] private Transform m_leftHandTarge = null;

    private Transform m_transform       = null;
    private Animator  m_myAnim          = null;   
    private Transform m_targetTransform = null;

    private float     m_ikLeftHandWeight = 1f;

    public bool  IsPlayerDetect { get; private set; }
    public bool  IsDead         { get { return m_hp <= 0f; } }

    public Animator Anim        { get { return m_myAnim; } }

	void Awake () {
        m_transform = transform;
        m_myAnim    = GetComponent<Animator>();
        m_targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}

    void Update()
    {
        var dir = (m_targetTransform.position - m_transform.position).normalized;
        var rot = Quaternion.LookRotation(dir);
        var rotY = rot.eulerAngles.y - m_transform.rotation.eulerAngles.y;
        Debug.Log(rotY);
        m_myAnim.SetFloat("horAngle", rotY);
    }
	
    void OnAnimatorIK(int layer)
    {
        m_ikLeftHandWeight = Mathf.MoveTowards(m_ikLeftHandWeight, 1f, Time.smoothDeltaTime * 2f);
        m_myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_ikLeftHandWeight);
        m_myAnim.SetIKPosition(AvatarIKGoal.LeftHand, m_leftHandTarge.position);
    }


    public void BeAttacked(float damage)
    {
        m_hp -= damage;
        IsPlayerDetect = true;
    }

    [Inspector(group = "AI Test")]
    private void InverseIsPlayerDetect()
    {
        IsPlayerDetect = !IsPlayerDetect;
    }

    [Inspector(group ="AI Test")]
    private void HpDecrease()
    {
        m_hp -= 5f;
    }
}
