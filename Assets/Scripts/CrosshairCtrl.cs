using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CrosshairCtrl : MonoBehaviour {
    [SerializeField] private Sprite    m_normalSprite  = null;
    [SerializeField] private Sprite    m_takeAimSprite = null;

    private Image     m_myImage;

	// Use this for initialization
	void Awake () {
        m_myImage            = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        var rayHitInfo = TPSCameraCtrl.Instance.CameraCenterRaycast;

        if (rayHitInfo.isHit)
        {
            RaycastHit rayHit = rayHitInfo.rayHit;
            if (rayHit.collider.tag == "AimTarget")
                m_myImage.sprite = m_takeAimSprite;
            else
                m_myImage.sprite = m_normalSprite;
        }
    }
}
