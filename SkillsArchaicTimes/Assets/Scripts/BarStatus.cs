using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarStatus : MonoBehaviour
{
    [SerializeField]
    private int numSegments = 32; // quality setting - the higher the better it looks in close-ups
    [Range(0f, 1f)]
    public float fillState = 1.0f; // how full the bar is
    public float modifier;

    // Called when you change something in the inspector 
    // or change the FillState via another script
    private void RecalculatePoints()
    {
        // calculate the positions of the points
        float angleIncrement = Mathf.PI * fillState / numSegments;
        float angle = 0.0f;
        Vector3[] positions = new Vector3[numSegments + 1];
        for (var i = 0; i <= numSegments; i++)
        {
            positions[i] = new Vector3(
                Mathf.Cos(angle),
                0.0f,
                Mathf.Sin(angle)
            );
            angle += angleIncrement;
        }
        // apply the new points to the LineRenderer
        LineRenderer myLineRenderer = GetComponent<LineRenderer>();
        myLineRenderer.positionCount = numSegments + 1;
        myLineRenderer.SetPositions(positions);
    }


    public void decrease(float amount)
    {
        fillState -= amount*modifier;
        if (fillState < 0.0f)
            fillState = 0.0f;
        else if (fillState > 1.0f)
            fillState = 1.0f;
        RecalculatePoints();
    }
}
