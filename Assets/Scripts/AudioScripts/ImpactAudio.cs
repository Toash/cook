
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Plays sound during rigidbody collision.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ImpactAudio : AudioSourcePlayer
{

    public override void Awake()
    {
        base.Awake();
        if (AudioDef == null)
        {
            AudioDef = Resources.Load<AudioDefinition>("ScriptableObjects/AudioDefinition/DefaultHit");
        }
        SetAudioDef(AudioDef);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2)
        {

            Play();

        }

    }

}