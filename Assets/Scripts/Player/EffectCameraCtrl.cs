using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

/**
 * @brief Effect 렌더링을 담당하는 카메라의 컨트롤러 스크립트
 */

[RequireComponent(typeof(Camera))]
public class EffectCameraCtrl : MonoBehaviour {
    [SerializeField] private Camera m_mainCamera = null;
    
    private Camera m_camera = null;

	void Awake () {
        m_camera = GetComponent<Camera>();

        this.ObserveEveryValueChanged(_  => m_mainCamera.fieldOfView)
            .Subscribe(fieldOfView => m_camera.fieldOfView = fieldOfView);
	}
}
