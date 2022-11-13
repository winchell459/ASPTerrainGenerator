using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObject/Ability")]
public class AbilityItem : ScriptableObject
{
    public enum AbilityTypes
    {
        DoubleJump,
        Dash,
        Hover,
        Fly,
        StopWatch,
        Climb
    }

    public AbilityTypes AbilityType;
    public float Power = 1;
    public float Duration = 1;
    private float usedStart = float.NegativeInfinity;
    private bool usedStarted = false;
    public bool isUnique = true;
    public bool isEquipable = true;

    public bool canUse
    {
        get
        {
            if (!usedStarted)
            {
                usedStart = Time.fixedTime;
                usedStarted = true;
                return true;
            }else if(Time.fixedTime < usedStart + Duration)
            {
                return true;
            }
            return false;
        }
    }

    public void ResetUse()
    {
        usedStarted = false;
    }
}
