﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * @brief UI 중 크로스헤어 이미지의 제어를 담당하는 컨트롤러 스크립트
 */

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
	void LateUpdate () {
        if (TPSCameraCtrl.Instance.IsRayHit)
        {
            RaycastHit rayHit = TPSCameraCtrl.Instance.RayHit;
            if (rayHit.collider)
            {
                if (rayHit.collider.tag == "Enemy")
                    m_myImage.sprite = m_takeAimSprite;
                else
                    m_myImage.sprite = m_normalSprite;
            }
        }
    }
}
