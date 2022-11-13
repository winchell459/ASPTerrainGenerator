using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Vector2 HeadPlacement;
    public Vector2 ThrowingVelocity = new Vector2(3, 1);
    public float mass = 1;
    private bool ignited = false;

    public List<GameObject> bombTargets = new List<GameObject>();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!ignited && collision.transform.CompareTag("Player") && !collision.transform.GetComponentInChildren<Bomb>())
        {
            ignited = true;
            transform.parent = collision.transform;
            transform.localPosition = HeadPlacement;
            //mass = GetComponent<Rigidbody2D>().mass;
            //Destroy(GetComponent<Rigidbody2D>());
            GetComponent<Animator>().SetTrigger("Ignite");

        }
    }

    public void Throw(Transform Thrower)
    {
        transform.parent = null;
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.mass = mass;

        //get velocity and direction of the player
        float dir = Mathf.Sign(Thrower.localScale.x);
        Vector2 ThrowerVelocity = Thrower.parent.GetComponent<Rigidbody2D>().velocity;

        
        rb.velocity = ThrowerVelocity + new Vector2(dir * ThrowingVelocity.x, ThrowingVelocity.y);

        Debug.Log("Thrown Bomb");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("BombWall"))
        {
            if(!bombTargets.Contains(collision.gameObject)) bombTargets.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BombWall"))
        {
            if(bombTargets.Contains(collision.gameObject)) bombTargets.Remove(collision.gameObject);
        }
    }

    public void BombExplodes()
    {
       
        Destroy(gameObject);
    }
}
