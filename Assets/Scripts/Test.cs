using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using UnityEngine.UI;

public class Test : MonoBehaviour {
    [SerializeField] private RectTransform rectTr;
    [SerializeField] private RectTransform leftScrollRectTansform;
    [SerializeField] private RectTransform rightScrollRectTansform;
    [SerializeField] private float rightCheckPointRivisionValue;
    [SerializeField] private float LeftCheckPointRivisionValue;

    float scrollDistance  = 0f;
    float leftCheckPoint  = 0f;
    float rightCheckPoint = 0f;

	// Use this for initialization
	void Awake () {
        scrollDistance  = Mathf.Abs(leftScrollRectTansform.localPosition.x - rightScrollRectTansform.localPosition.x);
        leftCheckPoint  = leftScrollRectTansform.position.x + LeftCheckPointRivisionValue;
        rightCheckPoint = rightScrollRectTansform.position.x + rightCheckPointRivisionValue;
        Debug.Log(rightCheckPoint);
        //this.ObserveEveryValueChanged(_ => leftScrollRectTansform.localPosition)
        //    .Where(_ => (leftScrollRectTansform.localPosition.x > rightCheckPointRivisionValue) || (leftScrollRectTansform.localPosition.x < LeftCheckPointRivisionValue))
        //    .Subscribe(_ => {
        //        if (leftScrollRectTansform.localPosition.x > rightCheckPointRivisionValue)
        //            leftScrollRectTansform.localPosition = new Vector3(leftCheckPoint, 0f, 0f);
        //        else if (leftScrollRectTansform.localPosition.x < rightCheckPointRivisionValue)
        //            leftScrollRectTansform.localPosition = new Vector3(rightCheckPoint, 0f, 0f);
        //    });
	}
	
	// Update is called once per frame
	void Update () {
        float rightPosX = rightScrollRectTansform.position.x;
        float leftPosX  = leftScrollRectTansform.position.x;
        float rightLocalPosX = rightScrollRectTansform.localPosition.x;
        float leftLocalPosX  = leftScrollRectTansform.localPosition.x;

        if (rightPosX > leftPosX)
        {
            if (leftPosX > rightCheckPoint)
                rightScrollRectTansform.localPosition = new Vector3(leftLocalPosX - scrollDistance, 0f, 0f);
            else if (rightPosX < leftCheckPoint)
                leftScrollRectTansform.localPosition = new Vector3(rightLocalPosX + scrollDistance, 0f, 0f);
        }
        else if (rightPosX < leftPosX)
        {
            if (rightPosX > rightCheckPoint)
                leftScrollRectTansform.localPosition = new Vector3(rightLocalPosX - scrollDistance, 0f, 0f);
            else if (leftPosX < leftCheckPoint)
                rightScrollRectTansform.localPosition = new Vector3(leftLocalPosX + scrollDistance, 0f, 0f);
        }
    }
}
