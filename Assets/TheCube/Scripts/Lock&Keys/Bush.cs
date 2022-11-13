using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public GameObject BombPrefab;
    public float GenerateTime = 2;
    private float generationBegan;
    private bool generationStarted;

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0 && !generationStarted)
        {
            generationBegan = Time.fixedTime;
            generationStarted = true;
        }else if(generationStarted && Time.fixedTime > generationBegan + GenerateTime)
        {
            generationStarted = false;
            Instantiate(BombPrefab, transform.position + new Vector3(0, -0.125f, 0), Quaternion.identity).transform.parent = transform;
        }
    }
}
