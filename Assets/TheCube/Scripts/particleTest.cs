using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class particleTest : MonoBehaviour
{
    ParticleSystem ps;
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    //public LayerMask mask;
    int colliderCount = 0;

    // Start is called before the first frame update
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        //Debug.Log(mask.value);
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        colliderCount = tilemaps.Length;
        for(int i = 0; i < tilemaps.Length; i += 1)
        {
            //Debug.Log(ps.trigger.GetCollider(i));
            ps.trigger.SetCollider(i, tilemaps[i].GetComponent<Collider2D>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnParticleTrigger()
    {
        
        int enterCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for(int i = 0; i < colliderCount; i += 1)
        {
            //Debug.Log(i + " " + colliderCount);
            if (!ps.trigger.GetCollider(i).gameObject.CompareTag("BombWall")) continue;
            Tilemap tilemap = ps.trigger.GetCollider(i).GetComponent<Tilemap>();
            foreach (ParticleSystem.Particle particle in enter)
            {
                Vector2 particlePos = particle.position + transform.position;
                //Debug.Log(particlePos);
                TilemapUtility.DestroyTile(tilemap, particlePos);
            }
        }
        
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Collision");
    }
}
