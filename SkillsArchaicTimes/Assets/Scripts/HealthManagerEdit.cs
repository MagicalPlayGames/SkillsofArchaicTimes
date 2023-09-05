using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HealthManagerEdit : MonoBehaviour
{
	public BodyHealth[] partHealths;
	public float totalHealth;
    public float damageMultiplier;
    public bool dead;
    public bool instantDeath;

    private EnemySpawnScript spawnScript;

    public EnemyANN annScript;
    // Start is called before the first frame update
    void Start()
    {
        //spawnScript = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawnScript>();
        foreach (BodyHealth partHP in partHealths)
		{
			totalHealth += partHP.health;
            partHP.damageMultipler = damageMultiplier;
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (instantDeath)
            die();
        if (partHealths[0] == null)
            die();
        else if(totalHealth<=0 || partHealths[1].health<=0)
        {
            die();
        }
    }

    public void takeDamage(float damage)
    {
        totalHealth -= damage;
        annScript.gotHit = 0;
    }

    public void die()
    {
        GetComponentInChildren<Animator>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        if (dead)
            return;
        dead = true;
        //spawnScript.disableEnemy(this.gameObject);
        if (partHealths[3] != null)
        {
            if (partHealths[3].gameObject.GetComponent<Dismemberer>() != null)
            {
                partHealths[3].gameObject.GetComponent<Dismemberer>().Dismember();
                Destroy(partHealths[3]);
            }
        }

        if (partHealths[7] != null)
        {
            if (partHealths[7].gameObject.GetComponent<Dismemberer>() != null)
            {
                partHealths[7].gameObject.GetComponent<Dismemberer>().Dismember();
                Destroy(partHealths[7]);
            }
        }

        if (partHealths[12] != null)
        {
            if (partHealths[12].gameObject.GetComponent<Dismemberer>() != null)
            {
                partHealths[12].gameObject.GetComponent<Dismemberer>().Dismember();
                Destroy(partHealths[12]);
            }
        }

        if (partHealths[17] != null)
        {
            if (partHealths[17].gameObject.GetComponent<Dismemberer>() != null)
            {
                partHealths[17].gameObject.GetComponent<Dismemberer>().Dismember();
                Destroy(partHealths[17]);
            }
        }

        if (partHealths[18] != null)
        {
            GameObject torso = partHealths[18].gameObject;
            torso.GetComponent<Rigidbody>().isKinematic = false;
            torso.GetComponent<XRGrabInteractable>().enabled = true;
            torso.GetComponent<Collider>().isTrigger = false;
            Destroy(partHealths[18]);
        }

        if (partHealths[19] != null)
        {
            GameObject torso = partHealths[19].gameObject;
            torso.GetComponent<Collider>().isTrigger = false;
            Destroy(partHealths[19]);
        }
        Destroy(this);
    }
}
