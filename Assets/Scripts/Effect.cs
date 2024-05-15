using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem JumpParticle;
    public ParticleSystem DashParticle;
    public ParticleSystem SlideParticle;

    private void Start()
    {
        
    }

    private void Update()
    {
    }

    public IEnumerator PlayDashParticle(float time)
    {
        DashParticle.Play();
        yield return new WaitForSeconds(time);
        DashParticle.Stop();
    }
}
