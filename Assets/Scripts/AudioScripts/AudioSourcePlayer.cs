using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Base class that automatically makes an AudioSource on the object. 
/// Uses AudioDefinition to play a sound, so defining a audio resource and source settings is data driven.
/// </summary>
[DisallowMultipleComponent]
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

        SetAudioDef(AudioDef);
    }

    public void SetAudioDef(AudioDefinition audio)
    {

        if (audio == null) return;

        // audioSource.volume = audio.Settings.Volume;
        // audioSource.loop = audio.Settings.Looping;
        // audioSource.spatialBlend = audio.Settings.SpatialBlend;
        // audioSource.resource = audio.Resource;

        // audioSource.minDistance = audio.Settings.MinDistance;


        audioSource.volume = audio.Volume;
        audioSource.loop = audio.Looping;
        audioSource.spatialBlend = audio.SpatialBlend;
        audioSource.minDistance = audio.MinDistance;

        audioSource.resource = audio.Resource;

    }

    public void Play()
    {
        SetAudioDef(AudioDef);
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }




}