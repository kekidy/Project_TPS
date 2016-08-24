using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
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
	
    void Update()
    {
        MoveUpdate();
        AttackUpdate();
    }

	void LateUpdate () {
       LookAt();
	}

    void OnAnimatorIK(int layer)
    {
        if (layer == 0)
        {
            m_myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);
            m_myAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        }
    }

    private void AttackUpdate()
    {
        if (Input.GetButton("Shot"))
            m_myAnim.SetBool("isShot", true);
        else
            m_myAnim.SetBool("isShot", false);
    }

    private void MoveUpdate()
    {
        float   vertical   = Input.GetAxis("Vertical");
        float   horizontal = Input.GetAxis("Horizontal");
        Vector3 direction  = ((Vector3.forward * vertical) + (Vector3.right * horizontal));
        float   inputMagnitude = direction.magnitude;

        if (inputMagnitude >= 0.5f)
            m_myAnim.SetFloat("moveStartAngle", Quaternion.LookRotation(direction, Vector3.up).eulerAngles.y);
        else
            m_myAnim.SetFloat("moveStopAngle", m_myAnim.GetFloat("moveStartAngle"));

        m_myAnim.SetFloat("vertical", vertical);
        m_myAnim.SetFloat("horizontal", horizontal);
        m_myAnim.SetFloat("inputMagnitude", inputMagnitude);
    }

    private void LookAt()
    {
        m_myTransform.eulerAngles = new Vector3(m_myTransform.eulerAngles.x, m_cameraTransform.eulerAngles.y, m_myTransform.eulerAngles.z);
    }


}
