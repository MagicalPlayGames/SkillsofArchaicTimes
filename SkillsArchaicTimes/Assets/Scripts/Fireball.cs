using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{


    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag != "Player" && !collision.gameObject.name.Contains("Controller"))
            blowup();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player" && !other.gameObject.name.Contains("Controller"))
            blowup();
    }

    void blowup()
    {
        Destroy(this.gameObject);
    }
}
