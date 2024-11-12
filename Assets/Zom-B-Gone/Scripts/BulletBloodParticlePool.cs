using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBloodParticlePool : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particlePool;

    public void SpawnEnterParticles(Transform bulletT)
    {
        ParticleSystem availableParticle = GetAvailableParticleSystem();
        if (availableParticle != null)
        {
            ActivateParticle(availableParticle, bulletT.position, bulletT.rotation);
        }
    }

    public void SpawnExitParticles(Transform bulletT)
    {
        ParticleSystem availableParticle = GetAvailableParticleSystem();
        if (availableParticle != null)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, bulletT.eulerAngles.z + 180);
            ActivateParticle(availableParticle, bulletT.position + (bulletT.up * 0.3f), rotation);
        }
    }

    private ParticleSystem GetAvailableParticleSystem()
    {
        foreach (var particle in particlePool)
        {
            if (!particle.isPlaying)
            {
                return particle;
            }
        }
        return null;
    }

    // test if setting active vs unactive helps with performance

    private void ActivateParticle(ParticleSystem particle, Vector3 position, Quaternion rotation)
    {
        particle.transform.SetPositionAndRotation(position, rotation);
        particle.gameObject.SetActive(true);
        particle.Play();
        StartCoroutine(DeactivateParticleWhenFinished(particle));
    }

    private IEnumerator DeactivateParticleWhenFinished(ParticleSystem particle)
    {
        yield return new WaitUntil(() => !particle.isPlaying);
        particle.Stop();
        particle.gameObject.SetActive(false);
    }
}
