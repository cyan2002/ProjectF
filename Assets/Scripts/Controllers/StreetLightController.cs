using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLightController : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D light;

    void Start()
    {
        if (Clock.Instance.timeOfDay > 7 && Clock.Instance.timeOfDay < 20)
        {
            light.enabled = false;
        }
        else
        {
            light.enabled = true;
        }
    }

    void OnEnable()
    {
        Clock.OnHourChanged += LightSwitch;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
