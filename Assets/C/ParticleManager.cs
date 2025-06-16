using System;
using UnityEngine;


[Serializable]
public class particleInfo
{
    public string name;
    public GameObject Particle;
}

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;
    [SerializeField]
    public particleInfo[] particles;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void InstantiateParticle(Vector3 pos, string name)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i].name == name)
            {
                GameObject particle = Instantiate(particles[i].Particle);
                particle.transform.position = pos;
            }


        }
    }


}
