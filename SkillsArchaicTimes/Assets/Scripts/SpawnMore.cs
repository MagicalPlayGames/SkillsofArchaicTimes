using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnMore : MonoBehaviour
{
    public GameObject[] toSpawn;
    public SpawnMore[] myNeighbors;

    void despawnOthers()
    {
        foreach(SpawnMore script in myNeighbors)
        {
            script.despawn();
        }
    }

    public void despawn()
    {
        foreach(GameObject Object in toSpawn)
        {
            if(Object.GetComponent<SpawnMore>()!=null)
            {
                foreach(GameObject obj in Object.GetComponent<SpawnMore>().toSpawn)
                {
                    obj.SetActive(false);
                }
            }
            Object.SetActive(false);
        }
    }

    public void spawnMore()
    {
        despawnOthers();
        foreach(GameObject Object in toSpawn)
        {
            Object.SetActive(true);
            Object.GetComponent<Spawned>().wakeUp();
            if(Object.GetComponent<SpawnMore>()!=false)
            {
                Object.GetComponent<SpawnMore>().despawnOthers();
            }
        }
    }
}
