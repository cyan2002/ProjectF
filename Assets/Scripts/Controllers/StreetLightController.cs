using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLightController : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D light;

    private void updateLights()
    {
        if (Clock.Instance.timeOfDay <= 7 || Clock.Instance.timeOfDay >= 18)
        {
            light.enabled = true;
        }
        else
        {
            light.enabled = false;
        }
    }

    void OnEnable()
    {
        Clock.OnHourChanged += LightSwitch;
        updateLights();
    }

    void OnDisable()
    {
        Clock.OnHourChanged -= LightSwitch;
    }

    void LightSwitch(int hour)
    {
        if (hour == 7) //lights off
        {
            light.enabled = false;
        }
        else if(hour == 18) //lights on
        {
            light.enabled = true;
        }
    }
}
