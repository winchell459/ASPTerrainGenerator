using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldHandler : MonoBehaviour
{
    Vector2 gravityDirection;
    Vector2 gravityDefault;
    private void Awake()
    {
        gravityDefault = Physics2D.gravity;
        gravityDirection = gravityDefault.normalized;
    }

    public void FlipGravity(Vector2 flipDirection)
    {
        gravityDirection = flipDirection.normalized;
        Physics2D.gravity = gravityDefault.magnitude * gravityDirection;
    }

    bool gravityFlipped;
    public void FlipGravity()
    {
        if (gravityFlipped)
        {
            FlipGravity(gravityDefault.normalized);
            gravityFlipped = false;
        }
        else
        {
            FlipGravity(-gravityDefault.normalized);
            gravityFlipped = true;
        }

        foreach(GravityObject go in FindObjectsOfType<GravityObject>())
        {
            go.FlipGravity();
        }
    }
}
