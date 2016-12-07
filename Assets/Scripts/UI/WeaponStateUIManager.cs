using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;
using EasyEditor;

public class WeaponStateUIManager : MonoBehaviour {
    [Inspector(group = "Target Info")]
    [SerializeField] private PlayerCtrl m_playerCtrl = null;

    [Inspector(group = "Weapon State UI")]
    [SerializeField] private Text m_magazineNumText = null;

    [Inspector(group = "Image Obj Info")]
    [Message(messageType = MessageType.Info, text = "Put the weapon image objs in enumeration order")]
    [SerializeField] private GameObject[] m_weaponImageObj = null;

    private GameObject m_currentImageObj = null;

    void Awake()
    {
        for (int i = 0; i < m_weaponImageObj.Length; i++)
            m_weaponImageObj[i].SetActive(false);

        m_currentImageObj = m_weaponImageObj[0];
    }

    void Start()
    {
        this.ObserveEveryValueChanged(_ => m_playerCtrl.CurrentGunBase)
            .Subscribe(gunbase =>
            {
                GameObject weaponImageObj = m_weaponImageObj[(int)gunbase.WeaponType];

                m_currentImageObj.SetActive(false);
                weaponImageObj.SetActive(true);
                m_currentImageObj = weaponImageObj;
            });

        this.ObserveEveryValueChanged(_ => m_playerCtrl.CurrentGunBase.CurrentMagazinNum)
            .Subscribe(magazineNum => {
                m_magazineNumText.text = magazineNum.ToString();
            });
    }
}