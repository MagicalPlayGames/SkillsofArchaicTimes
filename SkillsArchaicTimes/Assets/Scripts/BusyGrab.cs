using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusyGrab : MonoBehaviour
{
    public MonoBehaviour[] scripts;
    public bool busyBody;
    /*BusyGrab decides when an interactor object is busy,
     * disabling most of its script
     */

    public void busy(MonoBehaviour usedScript)
    {
        if (!busyBody)
        { 
            foreach (MonoBehaviour script in scripts)
            {
                if (usedScript != script)
                    script.enabled = false;
            }
        busyBody = true;
    }
    }

    public void free()
    {
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
        busyBody = false;
    }


}
