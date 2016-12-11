using UnityEngine;
using System.Collections;

/**
 * @brief Cursor의 Visible을 제어하기 위한 컨트롤러 스크립트
 */

public class CursorCtrl : MonoBehaviour {
	void Awake () {
        Cursor.visible = false;
	}
}
