using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLightController : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D streetLight;

    //method only used once when intitializing the scene
    private void updateLights()
    {
        if (Clock.Instance.timeOfDay <= 7 || Clock.Instance.timeOfDay >= 18)
        {
            streetLight.enabled = true;
        }
        else
        {
            streetLight.enabled = false;
        }
    }

    void OnEnable()
    {
        Clock.OnHourChanged += LightSwitch;
        streetLight = this.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        updateLights();
    }

    void OnDisable()
    {
        Clock.OnHourChanged -= LightSwitch;
    }

    void LightSwitch(int hour)
    {
        if (hour <= 7) //lights off
        {
            streetLight.enabled = false;
        }
        else if(hour >= 18) //lights on
        {
            streetLight.enabled = true;
        }
    }
}
