using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    // References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    public GameObject UIDocument;
    private ChangeLightButton light_script;
    
    // Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    public List<Light> Lights;
    
    private void Update()
    {
        if (Preset == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime;
            TimeOfDay %= 24; // Clamp between 0-24
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        // Update On/Off/Dim
        light_script = UIDocument.GetComponent<ChangeLightButton>();
        if (light_script.click == 1)
        {
            Lights.ForEach(light =>light.enabled = false);
        }

        if (light_script.click == 2)
        {
            Lights.ForEach(light =>light.enabled = true);
            Lights.ForEach(light =>light.GetComponent<Light>().intensity = 5);
        }

        if (light_script.click == 0)
        {
            Lights.ForEach(light =>light.GetComponent<Light>().intensity = 2);
        }
        
        // Update for DayNightCycle
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.Fogcolor.Evaluate(timePercent);
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation =
                Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0));
        }
    }

    // Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
        {
            return;
        }

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}