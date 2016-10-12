using UnityEngine;
using System.Collections;

public class RunnerBotCtrl : MonoBehaviour {
    [SerializeField] private Transform leftHandTarget;

    private Animator m_myAnim = null;

    private float m_ikLeftHandWeight = 1f;

	void Awake () {
        m_myAnim = GetComponent<Animator>();
	}
	

	void Update () {
	
	}

    void OnAnimatorIK(int layer)
    {
        m_ikLeftHandWeight = Mathf.MoveTowards(m_ikLeftHandWeight, 1f, Time.smoothDeltaTime * 2f);
        m_myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_ikLeftHandWeight);
        m_myAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
    }
}
