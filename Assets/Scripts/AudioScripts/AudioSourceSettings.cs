using UnityEngine;

/// <summary>
/// Deprecated
/// </summary>
[CreateAssetMenu(fileName = "AudioSourceSettings", menuName = "MyAudio/AudioSourceSettings")]
public class AudioSourceSettings : ScriptableObject
{

    public float Volume = 1f;
    public float SpatialBlend = 1f;
    public float MinDistance = 5f;
    public bool Looping = false;


}