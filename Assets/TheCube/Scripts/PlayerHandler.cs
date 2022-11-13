using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public PlayerInventory Inventory;
    public bool ResetOnStart;
    private void Awake()
    {
        if (ResetOnStart)
        {
            //Inventory.hasDoubleJump = false;
            Inventory.hasYellowKey = false;
        }
    }
}
