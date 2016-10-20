using UnityEngine;
using System.Collections;
using EasyEditor;

public class LightCtrl : MonoBehaviour {
    Light[] lights;

    [Inspector(group = "")]
    public void LightUpdate()
    {
        lights = FindObjectsOfType<Light>();
    }

    [Inspector(group = "")]
    public void BakeLightOff()
    {
        foreach(var light in lights)
        {
            if (light.isBaked)
                light.gameObject.SetActive(false);
        }
    }

    [Inspector(group = "")]
    public void BakeLightOn()
    {
        foreach (var light in lights)
        {
            if (light.isBaked)
                light.gameObject.SetActive(true);
        }
    }

    [Inspector(group = "")]
    public void BakeLightSettingChange()
    { 
        foreach (var light in lights)
        {
            if (light.range == 6f && light.intensity == 2.8f)
            {
                light.range = 6f;
                light.intensity = 1.8f;
            }
        }
    }
}
