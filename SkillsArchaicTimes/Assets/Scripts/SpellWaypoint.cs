using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellWaypoint : MonoBehaviour
{
    public bool wayPointHit;
    public SpellWaypoint previousWayPoint;
    public SpellWaypoint nextWayPoint;
    public SpellRune mainScript;
    public GameObject firstLight;
    public GameObject secondLight;
    public bool testHit;

    /*Each waypoint sees the next and previous waypoints, and the SpellRune script
     * wayPointHit is only true when previousWayPoint was hit before this SpellWayPoint
     * When the last wayPointHit is true, SpellRune.drawn = true
     */
    void Update()
    {
        if(nextWayPoint==null && wayPointHit)
        {
            mainScript.draw();
        }
        if (previousWayPoint != null)
        {
            if (previousWayPoint.wayPointHit)
                highlight();
        }
        else
            highlight();

        if(testHit)
        {
            if (previousWayPoint != null)
            {
                if (previousWayPoint.wayPointHit)
                {
                    wayPointHit = true;
                    lightup();
                }
            }
            else
            {
                //mainScript.DestroyOthers();
                mainScript.turnOn();
                wayPointHit = true;
                lightup();
            }
        }
    }

    private void highlight()
    {
        firstLight.SetActive(true);
    }

    private void lightup()
    {
        secondLight.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == mainScript.Hand)
        {
            if (previousWayPoint != null)
            {
                if (previousWayPoint.wayPointHit)
                {
                    wayPointHit = true;
                    lightup();
                }
            }
            else
            {
                mainScript.DestroyOthers();
                mainScript.turnOn();
                wayPointHit = true;
                lightup();
            }
        }
    }
}
