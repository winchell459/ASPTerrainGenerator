using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInput<T> 
{
    public T value;
    public bool hold;
    public bool down;
    public bool up;
    
    public float downTime;
    public float upTime;

    private float threshold;

    public float upDuration { get { return downTime - upTime < 0 ? Time.time - upTime: downTime - upTime ; } } 
    public float downDuration { get { return upTime - downTime < 0 ? Time.time - downTime: upTime - downTime; } } // uptime - downtime

    public ButtonInput(float threshold)
    {
        this.threshold = threshold;
    }

    public void UpdateButton(T value, bool condition)
    {
        this.value = value;
        if (condition)
        {
            if (!hold)
            {
                down = true;
                downTime = Time.time;
            }
            else
            {
                down = false;
            }
            hold = true;
            up = false;
        }
        else
        {
            if (hold)
            {
                up = true;
                upTime = Time.time;
            }
            else
            {
                up = false;
            }
            hold = false;
            down = false;
        }
    }

    public void UpdateButton(T value)
    {
        bool condition = false;
        if (typeof(T) == typeof(bool)) condition = valueMag((bool)(object)value);
        else if (typeof(T) == typeof(float)) condition = valueMag((float)(object)value);
        else if (typeof(T) == typeof(Vector2)) condition = valueMag((Vector2)(object)value);
        UpdateButton(value, condition);
    }

    bool valueMag(bool value)
    {
        return value;
    }
    bool valueMag(float value)
    {
        return Mathf.Abs(value) > threshold;
    }
    bool valueMag(Vector2 value)
    {
        return value.magnitude > threshold;
    }
}
