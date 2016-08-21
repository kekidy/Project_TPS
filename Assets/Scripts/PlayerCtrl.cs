using UnityEngine;
using System.Collections;

public class PlayerCtrl : MonoBehaviour {
    [Header("IK Target")]
    [SerializeField] private Transform leftHandTarget;

	private Transform m_myTransform;
    private Animator  m_myAnim;

    private Transform m_cameraTransform;

	void Awake () {
        m_myTransform = transform;
        m_myAnim      = GetComponent<Animator>();
        m_cameraTransform = Camera.main.transform;
	}
	
	void LateUpdate () {
        m_myTransform.eulerAngles = new Vector3(m_myTransform.eulerAngles.x, m_cameraTransform.eulerAngles.y, m_myTransform.eulerAngles.z);
	}

    void OnAnimatorIK(int layer)
    {
        if (layer == 0)
        {
            m_myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);
            m_myAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        }
    }
}
