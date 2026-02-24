using UnityEngine;

[CreateAssetMenu(fileName = "AudioSourceSettings", menuName = "MyAudio/AudioSourceSettings")]
public class AudioSourceSettings : ScriptableObject
{

    public float Volume = 1f;
    public float SpatialBlend = 1f;
    public bool Looping = false;


}