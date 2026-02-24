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

    [Tooltip("Should be looping, set to a default value if not set.")]
    public ParticleSystem SizzlingEffectPrefab;
    Collider col;

    // keeps track of manager instances to destroy when needed.
    private Dictionary<Cookable, AudioSource> sizzleSounds = new Dictionary<Cookable, AudioSource>();
    private Dictionary<Cookable, ParticleSystem> sizzleEffects = new Dictionary<Cookable, ParticleSystem>();


    void OnValidate()
    {
        if (SizzleAudio == null)
        {
            SizzleAudio = Resources.Load<AudioDefinition>("ScriptableObjects/AudioDefinition/DefaultSizzle");
        }
        if (SizzlingEffectPrefab == null)
        {
            SizzlingEffectPrefab = Resources.Load<ParticleSystem>("ParticleSystem/DefaultSizzle");
        }

    }
    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            AudioSource audioSource = AudioManager.I.PlayLooping(SizzleAudio, cookable.transform.position);
            sizzleSounds.Add(cookable, audioSource);

            ParticleSystem sizzleEffect = ParticleManager.I.InstantiateAndPlay(SizzlingEffectPrefab, cookable.transform.position, Quaternion.LookRotation(Vector3.up, Vector3.up));
            sizzleEffects.Add(cookable, sizzleEffect);

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

            Destroy(sizzleEffects[cookable].gameObject);
            sizzleEffects.Remove(cookable);

        }
    }

}