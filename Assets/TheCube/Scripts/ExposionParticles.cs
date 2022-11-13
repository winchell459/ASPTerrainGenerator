using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExposionParticles : MonoBehaviour
{
    ParticleSystem ps;
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    int colliderCount = 0;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        colliderCount = tilemaps.Length;
        for(int i = 0; i < colliderCount; i += 1)
        {
            ps.trigger.SetCollider(i, tilemaps[i].GetComponent<Collider2D>());
        }

    }

    private void OnParticleTrigger()
    {
        int enterCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for(int i = 0; i < colliderCount; i += 1)
        {
            if (!ps.trigger.GetCollider(i).gameObject.CompareTag("BombWall")) continue;
            Tilemap tilemap = ps.trigger.GetCollider(i).GetComponent<Tilemap>();
            foreach(ParticleSystem.Particle particle in enter)
            {
                Vector2 particlePos = particle.position + transform.position;
                UtilityTilemap.DestroyTile(tilemap, particlePos);
            }
        }
    }
}
