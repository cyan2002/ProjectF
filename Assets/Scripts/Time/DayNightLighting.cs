using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // For Light2D — remove if using 3D

public class DayNightLighting : MonoBehaviour
{
    [Header("References")]
    public Clock clock;
    public Light2D globalLight;   // swap for public Light globalLight; if 3D

    [Header("Intensity")]
    public AnimationCurve intensityCurve = AnimationCurve.Linear(0, 0, 24, 1);

    [Header("Color")]
    public Gradient lightColorGradient;

    void Update()
    {
        if (clock == null || globalLight == null) return;

        float t = clock.timeOfDay;              // 0 – 24
        float t01 = t / 24f;                    // 0 – 1  (for Gradient)

        globalLight.intensity = intensityCurve.Evaluate(t);
        globalLight.color = lightColorGradient.Evaluate(t01);
    }
}