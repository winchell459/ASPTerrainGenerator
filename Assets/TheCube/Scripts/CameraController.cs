using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Smoothing = 1f;
    public Transform Target;
    public Vector3 Offset;

    public bool SnappingMode;
    public bool CameraBound;
    public Bound CurrentBound;

    public Vector2 Origin;
    public Vector2 GridSize = new Vector2(24, 13.5f);
    Vector2 gridOffset;

    public float TransitionSpeed = 25;
    // Start is called before the first frame update
    void Start()
    {
        Origin = transform.position;
        gridOffset = new Vector2(Origin.x - GridSize.x/2, Origin.y - GridSize.y/2);
        Offset = Target.position - transform.position;
        float height = 2 * GetComponent<Camera>().orthographicSize;
        float width = GetComponent<Camera>().aspect * height;
        GridSize = new Vector2(width, height);
    }

    private void FixedUpdate()
    {
        if (!SnappingMode)
        {
            Vector2 destination = Vector2.MoveTowards(transform.position, Target.position, Smoothing * Time.deltaTime);// - (Vector2)Offset;
            transform.position = new Vector3(destination.x, destination.y, transform.position.z);// - Offset;
        }
        else
        {
            Vector3 destintation = transform.position;
            if (CameraBound)
            {
                int gridH = 0;
                int gridW = 0;
                bool code1 = true;
                if (code1)
                {
                    gridH = (int)((-Origin.y + Target.position.y + GridSize.y / 2) / GridSize.y);
                    gridW = (int)((-Origin.x + Target.position.x + GridSize.x / 2) / GridSize.x);
                    if (-Origin.y + Target.position.y + GridSize.y / 2 < 0) gridH -= 1;
                    if (-Origin.x + Target.position.x + GridSize.x / 2 < 0) gridW -= 1;
                    //Debug.Log(gridH + " " + gridW);
                    //destintation = new Vector3(-Origin.x + gridW * GridSize.x + GridSize.x / 2, -Origin.y + gridH * GridSize.y, transform.position.z);
                }
                else
                {
                    float targetH = (Target.position.y - gridOffset.y);
                    float targetW = (Target.position.x - gridOffset.x);
                    gridH = targetH >= 0 ? (int)(targetH / GridSize.y) : (int)(targetH / GridSize.y) - 1;
                    gridW = targetW >= 0 ? (int)(targetW / GridSize.x) : (int)(targetW / GridSize.x) - 1;
                    //Debug.Log(targetH + " = " + gridW + " " + targetW + " = " + gridH);
                }

                destintation = new Vector3(Origin.x + gridW * GridSize.x , Origin.y + gridH * GridSize.y, transform.position.z);
            }
            else if (CurrentBound)
            {
                destintation = new Vector3(CurrentBound.transform.position.x, CurrentBound.transform.position.y, transform.position.z);
                
            }
            transform.position = Vector3.MoveTowards(transform.position, destintation, TransitionSpeed * Time.deltaTime);
        }
        
    }
}
