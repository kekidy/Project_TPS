using UnityEngine;
using System.Collections;
using UnityEditor;
using UniLinq;

public class LightController : EditorWindow {
    [MenuItem("Window/Light Controller")]
    static public void ShowWindow()
    {
        LightController lightCtrl = EditorWindow.GetWindow<LightController>() as LightController;
    }

    private Light[] m_lights;
    private ReflectionProbe[] m_refProbes;
    private int m_currentLightNum;
    private int m_currentRefProbeNum;

    void OnGUI()
    {
        GUILayout.Label("Current Activate Light Num : " + m_currentLightNum);
        GUILayout.Label("Current Ref Probe Num : " + m_currentRefProbeNum);

        if (GUILayout.Button("Light Update"))
            LightUpdate();

        if (GUILayout.Button("Ref Probe Update"))
            RefProbeUpdate();

        if (GUILayout.Button("Bake Light On"))
            BakeLightOn();

        if (GUILayout.Button("Bake Light Off"))
            BakeLightOff();

        if (GUILayout.Button("Bake Ref Probe On"))
            BakeRefProbeOn();

        if (GUILayout.Button("Bake Ref Probe Off"))
            BakeRefProbeOff();

        if (GUILayout.Button("Light Mode Change"))
            LightModeChange();

        if (GUILayout.Button("Scale In Lightmap Change"))
            ScaleInLightMapChange();

        if (GUILayout.Button("Destroy Light Flicker"))
            DestroyLightFlicker();

        if (GUILayout.Button("Change Light Flicker Value"))
            ChangeLightFlickerValue();
    }

    public void LightUpdate()
    {
        m_lights = FindObjectsOfType<Light>();
        m_currentLightNum = m_lights.Length;
    }

    public void RefProbeUpdate()
    {
        m_refProbes = FindObjectsOfType<ReflectionProbe>();
        m_currentRefProbeNum = m_refProbes.Length;
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

    public void BakeRefProbeOn()
    {
        foreach (var probe in m_refProbes)
        {
            probe.gameObject.SetActive(true);
        }
    }

    public void BakeRefProbeOff()
    {
        foreach (var probe in m_refProbes)
        {
            probe.gameObject.SetActive(false);
        }
    }

    public void LightModeChange()
    {
        Selection.objects = m_lights.Where(light => { return  !light.isBaked && light.GetComponent<LightFilker>() == null; }).ToArray();
    }

    public void ScaleInLightMapChange()
    {
        var renderers = FindObjectsOfType<Renderer>();

        foreach(var renderer in renderers)
        {
            var serializeObject = new SerializedObject(renderer);
            serializeObject.FindProperty("m_ScaleInLightmap").floatValue = 0.5f;
            serializeObject.ApplyModifiedProperties();
            Debug.Log(renderer.gameObject.name + "::ScaleInLightmap : " + serializeObject.FindProperty("m_ScaleInLightmap").floatValue);
        }
    }

    public void DestroyLightFlicker()
    {
        var flickers = FindObjectsOfType<LightFlickerPulse>();
        Debug.Log(flickers.Length);
        foreach (var flicker in flickers)
        {
            flicker.gameObject.AddComponent<LightFilker>();
            DestroyImmediate(flicker);
            
        }
    }

    public void ChangeLightFlickerValue()
    {
        var flickers = FindObjectsOfType<LightFilker>();

        foreach(var flicker in flickers)
        {
            if (flicker.CoreBase == 0f && flicker.Amplitude == 0f)
            {
                flicker.CoreBase  = 0.4f;
                flicker.Amplitude = 0.8f;
            }
        }
    }
}
