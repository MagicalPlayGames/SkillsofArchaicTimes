using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerHealth : MonoBehaviour
{
    public BarStatus healthBar;
    public EnemySpawnScript spawnScript;

    public GameObject xrRig;
    public GameObject lController;
    public GameObject rController;

    public GameObject endingObject;
    public GameObject endScreen;

    public void decrease(float amount)
    {
        healthBar.decrease(amount);
        if (healthBar.fillState <= 0.0f)
            die();
    }

    void die()
    {
        spawnScript.die();
        Destroy(GetComponent<PlayerMana>().manaBar);
        lController.GetComponent<SpellHandSelect>().enabled = false;
        rController.GetComponent<SpellHandSelect>().enabled = false;
        GameObject end = Instantiate(endingObject, this.transform.position, this.transform.rotation, null);
        end.GetComponentInChildren<EndingScript>().counter = spawnScript.counter;
        endScreen.SetActive(true);
        Destroy(this);
    }
}
