using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoryItem : MonoBehaviour
{
    public AbilityItem Ability;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            FindObjectOfType<PlayerHandler>().Inventory.AddAccessory(Ability);
            Destroy(gameObject);
        }
    }
}
