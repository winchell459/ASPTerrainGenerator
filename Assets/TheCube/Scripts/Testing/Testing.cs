using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Ball ballA = new Ball();
        Ball ballB = new Ball();

        ballA.name = "Nathan";
        ballB.name = "Richard";
        ballB.size = 5;
        ballA.size = 10;

        ballB.weight = 80;

        Debug.Log(ballA.name);
        Debug.Log(ballB.weight);

        if (Basket.makeBasket(ballA))
        {
            Debug.Log(ballA.name + " made a basket.");
        }
        else
        {
            Debug.Log(ballA.name + " missed the basket.");
        }

        string results = Basket.makeBasket(ballB) ? " made a basket." : " missed the basket.";
        Debug.Log(ballB.name + results);

        Debug.Log(ballB.name + (Basket.makeBasket(ballB) ? " made a basket." : " missed the basket."));
    }

    
}

class Ball
{
    public float size = 0;
    public float weight = 0;
    public string name = "";



}

static class Basket
{
    public static float radius = 5;

    public static bool makeBasket (Ball ball)
    {
        if(radius >= ball.size)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

