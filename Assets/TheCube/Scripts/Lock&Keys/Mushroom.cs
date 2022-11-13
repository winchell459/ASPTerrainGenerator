using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            //FindObjectOfType<PlayerHandler>().Inventory.hasDoubleJump = true;
            Destroy(gameObject);
        }
    }
}
