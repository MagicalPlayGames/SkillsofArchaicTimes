using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] spawnables;
    public GameObject[] curEnemies;
    public GameObject[] curWeapons;
    public GameObject[] deadEnemies;
    public Vector3 maxPos;
    public Vector3 minPos;
    public GameObject player;
    public float minBehindPlayerAngle;
    public float minSpawnDistance;
    public int maxWeapons;
    public int maxEnemies;
    public int maxDeadEnemies;
    public float spawnRechargeTime;
    private int curEnemiesSize;
    private int curWeaponSize;
    public int counter = 0;
    private int curDeadEnmies;

    private void Start()
    {

        StartCoroutine(spawnWait());
    }

    private void Update()
    {
        maxEnemies = (counter/3) + 1;
        if (curWeaponSize > maxWeapons)
        {
            for (int i = curWeapons.Length-1; i > 0; i--)
            {
                if (curWeapons[i] != null)
                {
                    if (curWeapons[i].GetComponent<WeaponData>() != null)
                    if (!curWeapons[i].GetComponent<WeaponData>().grabbed && (behindPlayer(curWeapons[i].transform.position) && farFromPlayer(curWeapons[i].transform.position) || curWeaponSize > 30))
                    {
                        if (curWeapons[i].transform.parent != null)
                            curWeapons[i] = curWeapons[i].transform.parent.gameObject;
                        Destroy(curWeapons[i]);
                            curWeaponSize--;
                    }
                }
                if (curWeaponSize < maxWeapons)
                    break;
            }
        }
        if (curDeadEnmies > maxDeadEnemies)
        {
            for (int i = deadEnemies.Length - 1; i > 0; i--)
            {
                if (deadEnemies[i] != null)
                {
                    if (behindPlayer(deadEnemies[i].transform.position))
                    {
                        Destroy(deadEnemies[i]);
                        deadEnemies[i] = null;
                        curDeadEnmies--;
                    }
                }
                if (curDeadEnmies <= maxDeadEnemies)
                    break;
            }
            if (curDeadEnmies > maxDeadEnemies)
                for (int i = deadEnemies.Length - 1; i > 0; i--)
                {
                    if (deadEnemies[i] != null)
                    {
                        Destroy(deadEnemies[i]);
                        deadEnemies[i] = null;
                        curDeadEnmies--;
                    }
                    if (curDeadEnmies <= maxDeadEnemies)
                        break;
                }
        }
    }

    IEnumerator spawnWait()
    {
        yield return new WaitForSeconds(spawnRechargeTime);
        if(curEnemiesSize<curEnemies.Length-1)
            chooseEnemy();
        StartCoroutine(spawnWait());
    }
    void spawn(Vector3 position, GameObject enemy)
    {
        addToList(Instantiate(enemy, position, enemy.transform.rotation));
    }

    void addToList(GameObject enemy)
    {
        for (int i = 0; i < curEnemies.Length; i++)
        {
            if (curEnemies[i] == null)
            {
                curEnemiesSize++;
                curEnemies[i] = enemy.transform.GetChild(0).gameObject;
                break;
            }
        }
    }

    void chooseEnemy()
    {
        curWeapons = GameObject.FindGameObjectsWithTag("Weapon");
        if (curEnemiesSize < maxEnemies && curEnemiesSize < curEnemies.Length - 1)
        {
            for (int i = -1; i < counter / 3; i++)
            {
                if (curEnemiesSize > maxEnemies || curEnemiesSize > curEnemies.Length - 2)
                    break;
                int enemyIndex = Random.Range(0, spawnables.Length);
                spawn(findPos(), spawnables[enemyIndex]);
            }
        }
        for(int i =0;i<curWeapons.Length;i++)
        {
            curWeaponSize = curWeapons.Length;
            WeaponData wD = null;
            if (curWeapons[i].transform.childCount > 0)
                if (curWeapons[i].GetComponentInChildren<WeaponData>() != null)
                    wD = curWeapons[i].GetComponentInChildren<WeaponData>();
                else if (curWeapons[i].GetComponent<WeaponData>() != null)
                    wD = curWeapons[i].GetComponent<WeaponData>();
                else if (curWeapons[i].GetComponent<WeaponData>() != null)
                    wD = curWeapons[i].GetComponent<WeaponData>();

            if (wD != null)
                if (wD.grabbed)
                {
                    curWeaponSize--;
                    curWeapons[i] = null;
                }
        }
        GameObject[] curShields = GameObject.FindGameObjectsWithTag("Shield");
        if(curShields.Length>3)
        {
            for(int i =0;i<2;i++)
            {
                Destroy(curShields[i]);
            }
        }
    }
    
    Vector3 findPos()
    {
        int i = 0;
        Vector3 pos = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), Random.Range(minPos.z, maxPos.z));
        while (!((behindPlayer(pos) && farFromPlayer(pos)) || i>20))
        {
            pos = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), Random.Range(minPos.z, maxPos.z));
            i++;
        }
        Debug.Log(pos);
        return pos;
    }

    bool behindPlayer(Vector3 pos)
    {
        Vector3 targetDirection = pos - player.transform.position;
        targetDirection.y = 0;
        bool should = Vector3.Angle(player.transform.forward, targetDirection) > minBehindPlayerAngle;
        return should;
    }

    bool farFromPlayer(Vector3 pos)
    {
        return Vector3.Distance(pos, player.transform.position) > minSpawnDistance;
    }

    void putInPosition(GameObject enemy)
    {
        for(int i =0;i<deadEnemies.Length;i++)
        {
            if (deadEnemies[i] == null)
            {
                deadEnemies[i] = enemy;
                return;
            }
        }
    }

    public void disableEnemy(GameObject thisChar)
    {
        for(int i =0;i<curEnemies.Length;i++)
        {
            if (thisChar == curEnemies[i])
            {
                putInPosition(curEnemies[i]);
                curEnemies[i] = null;
                curDeadEnmies++;
                curEnemiesSize--;
                counter++;
                return;
            }
        }
    }

    public void die()
    {
        int i = 0;
        while (i < curEnemies.Length || i < deadEnemies.Length || i < curWeapons.Length)
        {
            if (i < curEnemies.Length)
                if (curEnemies[i] != null)
                    Destroy(curEnemies[i]);

            if (i < deadEnemies.Length)
                if (deadEnemies[i] != null)
                    Destroy(deadEnemies[i]);

            if (i < curWeapons.Length)
                if (curWeapons[i] != null)
                    Destroy(curWeapons[i]);
            i++;
        }
        Destroy(this);
    }
}
