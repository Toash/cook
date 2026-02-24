using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager I;

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

    /// <summary>
    /// Particle prefab should be prefab reference to a particle system. 
    /// </summary>
    /// <param name="particlePrefab"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public ParticleSystem InstantiateAndPlay(ParticleSystem particlePrefab, Vector3 pos, Quaternion rot)
    {
        ParticleSystem inst = Instantiate(particlePrefab, pos, rot);
        inst.Play();
        return inst;
    }




}