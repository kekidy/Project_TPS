using UnityEngine;
using System.Collections;
using UnityEditor;

public class LightController : EditorWindow {
    [MenuItem("Window/Light Controller")]
    static public void ShowWindow()
    {
        LightController lightCtrl = EditorWindow.GetWindow<LightController>() as LightController;
    }

    private Light[] m_lights;
    private int m_currentLightNum;

    void OnGUI()
    {
        GUILayout.Label("Current Activate Light Num : " + m_currentLightNum);

        if (GUILayout.Button("Light Update"))
            LightUpdate();

        if (GUILayout.Button("Bake Light On"))
            BakeLightOn();

        if (GUILayout.Button("Bake Light Off"))
            BakeLightOff();
    }

    public void LightUpdate()
    {
        m_lights = FindObjectsOfType<Light>();
        m_currentLightNum = m_lights.Length;
    }

    public void BakeLightOff()
    {
        foreach (var light in m_lights)
        {
            if (light.isBaked)
                light.gameObject.SetActive(false);
        }
    }

    public void BakeLightOn()
    {
        foreach (var light in m_lights)
        {
            if (light.isBaked)
                light.gameObject.SetActive(true);
        }
    }
}
