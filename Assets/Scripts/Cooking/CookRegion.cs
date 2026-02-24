using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A region that can cook a cookable based on some type
/// </summary>
[RequireComponent(typeof(Collider))]
public class CookRegion : MonoBehaviour
{

    public AudioDefinition SizzleAudio;

    [Tooltip("The type that this region cooks.")]
    public CookType CookType;
    Collider col;

    private Dictionary<Cookable, AudioSource> sizzleSounds = new Dictionary<Cookable, AudioSource>();


    void Awake()
    {
        col = GetComponent<Collider>();
        if (SizzleAudio == null)
        {
            SizzleAudio = Resources.Load<AudioDefinition>("ScriptableObjects/AudioDefinition/DefaultSizzle");
        }
        col.isTrigger = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            AudioSource audioSource = AudioManager.I.PlayLooping(SizzleAudio, cookable.transform.position);
            sizzleSounds.Add(cookable, audioSource);
            cookable.SizzlingEffect.Play();
        }
    }
    void OnTriggerStay(Collider other)
    {

        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            cookable.Cook(Time.fixedDeltaTime);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            Destroy(sizzleSounds[cookable].gameObject);
            sizzleSounds.Remove(cookable);
            cookable.SizzlingEffect.Stop();

        }
    }

}