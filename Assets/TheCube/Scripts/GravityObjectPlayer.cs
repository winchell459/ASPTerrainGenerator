using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObjectPlayer : GravityObject
{
    private Transform[] isGroundedCheckers;
    public int gravityFlipped = 1;
    public override void FlipGravity()
    {
        gravityFlipped *= -1;
        foreach (Transform checker in isGroundedCheckers)
        {
            checker.localPosition = new Vector3(checker.localPosition.x, -checker.localPosition.y);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isGroundedCheckers = GetComponent<PlatformerController>().isGroundedCheckers;
    }

    
}
