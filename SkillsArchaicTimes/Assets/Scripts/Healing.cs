using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{

    public PlayerHealth healthBar;
    public float healRate;
    public SpellHandSelect hand;
    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.decrease(-healRate);
        hand.decreaseMana(2);
    }
}
