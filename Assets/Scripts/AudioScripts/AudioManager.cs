using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        else if (I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
        }
    }


    public void PlayOneShot(AudioDefinition audio, Vector3 pos = default)
    {
        GameObject audioObj = new GameObject("Audio Oneshot");
        AudioSourcePlayer player = audioObj.AddComponent<AudioSourcePlayer>();
        player.SetAudioDef(audio);

        audioObj.transform.position = pos;
        player.Play();

        StartCoroutine(DestroyWhenFinished(player.GetAudioSource()));
    }

    /// <summary>
    /// plays a looping audio source, returns AudioSource so its gameobject can be destroyed as needed.
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public AudioSource PlayLooping(AudioDefinition def, Vector3 pos)
    {
        GameObject audioObject = new GameObject("Looping audio");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();

        // audioSource.volume = def.Settings.Volume;
        // audioSource.spatialBlend = def.Settings.SpatialBlend;
        audioSource.volume = def.Volume;
        audioSource.spatialBlend = def.SpatialBlend;
        audioSource.playOnAwake = false;

        audioSource.loop = true;
        audioSource.resource = def.Resource;

        audioObject.transform.position = pos;
        audioSource.Play();

        return audioSource;
    }

    IEnumerator DestroyWhenFinished(AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return null;
        }

        Destroy(source.gameObject);
    }


}