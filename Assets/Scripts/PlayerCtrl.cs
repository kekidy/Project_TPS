using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCtrl : MonoBehaviour {
    [SerializeField] private RifleCtrl m_rifleCtrl;

    [Header("IK Target")]
    [SerializeField] private Transform leftHandTarget;

	private Transform m_myTransform;
    private Animator  m_myAnim;
    private Transform m_cameraTransform;

    private float m_ikLeftHandWeight = 1f;

	void Awake () {
        m_myTransform = transform;
        m_myAnim      = GetComponent<Animator>();
        m_cameraTransform = Camera.main.transform;
	}

    void Update()
    {
        MoveUpdate();
        AttackUpdate();
        SprintUpdate();
        ReloadUpdate();
    }

	void LateUpdate () {
       LookAt();
	}

    void OnAnimatorIK(int layer)
    {
        if (!m_myAnim.GetBool("isReload") && !m_myAnim.GetBool("isVault"))
        {
            m_ikLeftHandWeight = Mathf.MoveTowards(m_ikLeftHandWeight, 1f, Time.smoothDeltaTime * 2f);
            m_myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_ikLeftHandWeight);
            m_myAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        }
        else
            m_ikLeftHandWeight = 0f;
    }

    void OnCollisionStay(Collision col)
    {
        if (col.collider.tag == "Vault")
        {
            if (!m_myAnim.GetBool("isVault") && Input.GetAxis("Vertical") > 0f)
                m_myAnim.SetTrigger("vaultTrigger");
        }
    }

    private void AttackUpdate()
    {
        if (!m_myAnim.GetBool("isReload"))
        {
            if (Input.GetButton("Shot"))
                StartToAttack();
            else
                StopToAttack();
        }
    }

    private void StartToAttack()
    {
        m_myAnim.SetBool("isShot", true);
        m_rifleCtrl.StartToShooting();
    }

    private void StopToAttack()
    {
        m_myAnim.SetBool("isShot", false);
        m_rifleCtrl.StopToShooting();
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

    private void SprintUpdate()
    {
        if (Input.GetButton("Sprint"))
            m_myAnim.SetBool("isSprint", true);
        else
            m_myAnim.SetBool("isSprint", false);
    }

    private void ReloadUpdate()
    {
        if (!m_myAnim.GetBool("isReload"))
        {
            if (Input.GetButtonDown("Reload") || (m_rifleCtrl.CurrentMagazinNum == 0))
            {
                m_myAnim.SetBool("isReload", true);
                m_myAnim.SetBool("isSprint", false);
                m_rifleCtrl.PlayMagazineReloadSound();
                m_rifleCtrl.StopToShooting();
            }
        }
    }

    private void MagazineReload()
    {
        m_rifleCtrl.MagazineReload();
    }

    private void LookAt()
    {
        if (!m_myAnim.GetBool("isVault"))
            m_myTransform.eulerAngles = new Vector3(m_myTransform.eulerAngles.x, m_cameraTransform.eulerAngles.y, m_myTransform.eulerAngles.z);
    }


}
