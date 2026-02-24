using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Base class that automatically makes an AudioSource on the object. 
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
        audioSource.spatialBlend = 1;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        Initialize(AudioDef);
    }

    public void Initialize(AudioDefinition audio)
    {

        if (audio == null) return;

        audioSource.volume = audio.Settings.Volume;
        audioSource.resource = audio.Resource;

    }

    public void Play()
    {
        audioSource.Play();
    }




}