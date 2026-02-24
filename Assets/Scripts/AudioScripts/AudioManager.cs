using System.Collections;
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


    public void PlayOneShot(AudioDefinition audio, Vector3 pos)
    {
        GameObject audioObj = new GameObject("Audio Oneshot");
        AudioSourcePlayer player = audioObj.AddComponent<AudioSourcePlayer>();
        player.Initialize(audio);

        audioObj.transform.position = pos;
        player.Play();

        StartCoroutine(DestroyWhenFinished(player.GetAudioSource()));
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