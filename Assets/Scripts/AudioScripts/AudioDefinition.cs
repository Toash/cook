
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Wrapper class for an audio resource and audio source setting.
/// </summary>
[CreateAssetMenu(fileName = "AudioDefinition", menuName = "MyAudio/AudioDefinition")]
public class AudioDefinition : ScriptableObject
{

    public AudioResource Resource;
    // public AudioSourceSettings Settings;
    public float Volume = 1f;
    public float SpatialBlend = 1f;
    public float MinDistance = 5f;
    public bool Looping = false;

}