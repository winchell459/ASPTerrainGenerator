using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class YellowLock : MonoBehaviour
{
    public bool UnlockAll;

    public string BlockType = "lock_yellow";
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //foreach (ContactPoint2D hit in collision.contacts)
        //{
        
         
        if (collision.gameObject.CompareTag("Player") )
        {
            if (UnlockAll )
            {
                if(FindObjectOfType<PlayerHandler>().Inventory.hasYellowKey) Destroy(gameObject);
            }
            else
            {
                ContactPoint2D[] contacts = new ContactPoint2D[10];
                for (int i = 0; i < collision.GetContacts(contacts); i += 1)
                {
                    ContactPoint2D hit = collision.GetContact(i);
                    Tilemap tilemap = GetComponent<Tilemap>();
                    Vector3 pos = new Vector3(hit.point.x + hit.normal.x * 0.1f, hit.point.y + hit.normal.y * 0.1f, 0);
                    //Vector3Int tilePos = tilemap.WorldToCell(pos);
                    //TileBase tileBase = tilemap.GetTile(tilePos);
                    //Debug.Log(tileBase.name);
                    //if (tileBase && tileBase.name == "lock_yellow" && FindObjectOfType<PlayerHandler>().Inventory.hasYellowKey)
                    //    tilemap.SetTile(tilePos, null);
                    
                    if(TilemapUtility.GetTile(tilemap, pos) && TilemapUtility.GetTile(tilemap, pos).name == BlockType && FindObjectOfType<PlayerHandler>().Inventory.hasYellowKey)
                    {
                        TilemapUtility.DestroyTile(tilemap, pos);
                    }

                    //TilemapUtility.DestroyTile(tilemap, pos, "lock_yellow", ref FindObjectOfType<PlayerHandler>().Inventory.hasYellowKey);
                    

                }
            }
            
            
        }
    }

    //private void OnParticleCollision(GameObject other)
    //{
    //    Debug.Log("triggered");
    //}
    //private void OnParticleTrigger()
    //{
    //    Debug.Log("triggered " + gameObject.name);
    //}
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log(collision.name);
    //}
}
