using System;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [Header("Curves")]
    /// <summary>
    /// Curve for the directional light rotation
    /// </summary>
    public AnimationCurve DirectionalLightRotationCurve;

    /// <summary>
    /// Curve for the ambient and reflective lighting
    /// </summary>
    public AnimationCurve LightingCurve;
    [Header("Sun")]
    /// <summary>
    /// The main sunlight in the scene.
    /// </summary>
    public Light DirectionalLight;

    // The X rotation values for the directional light at the start and end of the day-night cycle
    public float StartXRotation = 50;
    public float EndXRotation = 230;
    float startingYRotation;
    float startingZRotation;
    float startingSunIntensity;

    // void Awake()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }
    // void OnEnable()
    // {
    //     DayManager.I.DayStarted += OnDayStarted;
    // }

    // void OnDisable()
    // {
    //     if (DayManager.I != null)
    //         DayManager.I.DayStarted -= OnDayStarted;
    // }
    void Start()
    {
        // cache daytime values for lerping
        startingYRotation = DirectionalLight.transform.rotation.eulerAngles.y;
        startingZRotation = DirectionalLight.transform.rotation.eulerAngles.z;
        startingSunIntensity = DirectionalLight.intensity;

    }
    // void OnDayStarted(int dayNumber)
    // {
    //     DirectionalLight =
    // }





    void Update()
    {
        float normalizedTime = TimeManager.I.GetNormalizedTime();
        float curveValue = DirectionalLightRotationCurve.Evaluate(normalizedTime);
        float lightingCurve = LightingCurve.Evaluate(normalizedTime);

        // Calculate the new X rotation based on the curve value
        float newXRotation = Mathf.Lerp(StartXRotation, EndXRotation, curveValue);
        DirectionalLight.transform.rotation = Quaternion.Euler(newXRotation, startingYRotation, startingZRotation);


        float newSunIntensity = lightingCurve * startingSunIntensity;
        DirectionalLight.intensity = newSunIntensity;

        RenderSettings.ambientIntensity = lightingCurve;
        RenderSettings.reflectionIntensity = lightingCurve;
    }


}
