using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 playerPos = collision.transform.position;
            float dx = Mathf.Abs(playerPos.x - transform.position.x);
            float dy = Mathf.Abs(playerPos.y - transform.position.y);
            if(dx < GetComponent<BoxCollider2D>().size.x/2 && dy < GetComponent<BoxCollider2D>().size.y / 2)
                FindObjectOfType<CameraController>().CurrentBound = this;
        }
    }
}
