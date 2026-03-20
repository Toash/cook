using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A region that cooks snapped cookables of a matching type.
/// </summary>
[RequireComponent(typeof(Snapper))]
public class CookRegion : MonoBehaviour
{
    public float CookRate = .3f;
    public AudioDefinition SizzleAudio;

    [Tooltip("The type that this region cooks.")]
    public CookType CookType;

    [Tooltip("Should be looping, set to a default value if not set.")]
    public ParticleSystem SizzlingEffectPrefab;

    private Snapper snapper;

    // active cookables currently being cooked in this region
    private readonly HashSet<Cookable> activeCookables = new();

    // keeps track of spawned runtime objects
    private readonly Dictionary<Cookable, AudioSource> sizzleSounds = new();
    private readonly Dictionary<Cookable, ParticleSystem> sizzleEffects = new();

    void InitRefs()
    {
        if (SizzleAudio == null)
        {
            SizzleAudio = Resources.Load<AudioDefinition>(
                "ScriptableObjects/AudioDefinition/DefaultSizzle"
            );
        }

        if (SizzlingEffectPrefab == null)
        {
            SizzlingEffectPrefab = Resources.Load<ParticleSystem>(
                "ParticleSystem/DefaultSizzle"
            );
        }

        if (snapper == null)
        {
            snapper = GetComponent<Snapper>();
        }
    }

    void OnValidate()
    {
        InitRefs();
    }

    void Awake()
    {
        InitRefs();
    }

    void OnEnable()
    {
        InitRefs();

        snapper.OnChildSnapped += HandleChildSnapped;
        snapper.OnChildDetached += HandleChildDetached;

        RefreshCookablesFromSnapHierarchy();
    }

    void OnDisable()
    {
        if (snapper != null)
        {
            snapper.OnChildSnapped -= HandleChildSnapped;
            snapper.OnChildDetached -= HandleChildDetached;
        }

        StopAllCookingEffects();
        activeCookables.Clear();
    }

    void FixedUpdate()
    {
        // cook everything currently snapped in this region
        foreach (Cookable cookable in activeCookables)
        {
            if (cookable == null) continue;
            if (cookable.CookType != CookType) continue;

            cookable.Cook(Time.fixedDeltaTime * CookRate);
        }
    }

    private void HandleChildSnapped(Snapper child)
    {
        RefreshCookablesFromSnapHierarchy();
    }

    private void HandleChildDetached(Snapper child)
    {
        RefreshCookablesFromSnapHierarchy();
    }

    /// <summary>
    /// Rebuilds the active cookable set from the Snapper hierarchy.
    /// </summary>
    private void RefreshCookablesFromSnapHierarchy()
    {
        HashSet<Cookable> newCookables = new();

        List<Snapper> snappedChildren = snapper.GetSnapperChildrenRecursive(includeSelf: false);

        foreach (Snapper childSnapper in snappedChildren)
        {
            if (childSnapper == null) continue;

            if (childSnapper.TryGetComponent<Cookable>(out var cookable))
            {
                if (cookable.CookType == CookType)
                {
                    newCookables.Add(cookable);
                }
            }
        }

        // start newly added cookables
        foreach (Cookable cookable in newCookables)
        {
            if (!activeCookables.Contains(cookable))
            {
                StartCookingEffects(cookable);
            }
        }

        // stop cookables no longer present
        List<Cookable> toRemove = new();
        foreach (Cookable cookable in activeCookables)
        {
            if (cookable == null || !newCookables.Contains(cookable))
            {
                toRemove.Add(cookable);
            }
        }

        foreach (Cookable cookable in toRemove)
        {
            StopCookingEffects(cookable);
        }

        activeCookables.Clear();
        foreach (Cookable cookable in newCookables)
        {
            activeCookables.Add(cookable);
        }
    }

    private void StartCookingEffects(Cookable cookable)
    {
        if (cookable == null) return;

        if (!sizzleSounds.ContainsKey(cookable))
        {
            AudioSource audioSource = AudioManager.I.PlayLooping(
                SizzleAudio,
                cookable.transform.position
            );

            sizzleSounds.Add(cookable, audioSource);
        }

        if (!sizzleEffects.ContainsKey(cookable))
        {
            ParticleSystem sizzleEffect = Instantiate(
                SizzlingEffectPrefab,
                cookable.transform.position,
                Quaternion.identity
            );

            sizzleEffect.transform.SetParent(cookable.transform, worldPositionStays: true);
            sizzleEffect.Play();

            sizzleEffects.Add(cookable, sizzleEffect);
        }
    }

    private void StopCookingEffects(Cookable cookable)
    {
        if (cookable == null) return;

        if (sizzleSounds.TryGetValue(cookable, out var audioSource))
        {
            if (audioSource != null)
            {
                Destroy(audioSource.gameObject);
            }

            sizzleSounds.Remove(cookable);
        }

        if (sizzleEffects.TryGetValue(cookable, out var effect))
        {
            if (effect != null)
            {
                Destroy(effect.gameObject);
            }

            sizzleEffects.Remove(cookable);
        }
    }

    private void StopAllCookingEffects()
    {
        foreach (var kvp in sizzleSounds)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value.gameObject);
            }
        }
        sizzleSounds.Clear();

        foreach (var kvp in sizzleEffects)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value.gameObject);
            }
        }
        sizzleEffects.Clear();
    }
}