using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Base class that automatically makes an AudioSource on the object. 
/// Uses AudioDefinition to play a sound, so defining a audio resource and source settings is data driven.
/// </summary>
public class AudioSourcePlayer : MonoBehaviour
{
    // public AudioSourceSettings Settings;
    // public AudioResource Resource;
    public AudioDefinition AudioDef;
    protected AudioSource audioSource;


    public AudioSource GetAudioSource()
    {
        return audioSource;
    }
    public virtual void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        Initialize(AudioDef);
    }

    public void Initialize(AudioDefinition audio)
    {

        if (audio == null) return;

        audioSource.volume = audio.Settings.Volume;
        audioSource.loop = audio.Settings.Looping;
        audioSource.spatialBlend = audio.Settings.SpatialBlend;
        audioSource.resource = audio.Resource;

    }

    public void Play()
    {
        audioSource.Play();
    }




}