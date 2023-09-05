using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBeam : MonoBehaviour
{
    public float damage;
    public float manaUsed;
    public SpellHandSelect hand;

    private void Update()
    {
        hand.decreaseMana(1);
    }
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<HealthManagerEdit>() != null)
        {
            other.gameObject.GetComponent<HealthManagerEdit>().partHealths[1].loseHealth(damage);
        }
    }
}
