using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBodyPart : MonoBehaviour
{
	public float health = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	// OnCollisionEnter called when objects are colliding
	// Can't get this one to work, says it works "if one of the colliders also has a non-kinematic rigidbody attached"
	// but wouldn't really work properly.
	void OnCollisionEnter(Collision collision)
	{
		//health -= 10.0f;
		Debug.Log(gameObject.name + " COLLISION!!");
	}
	
	// OnTriggerEnter called when collider is triggered
	// I think this is the one that is actually called?
	// For this to work, if you have the sword and body part,
	// then the sword must have a rigidbody
	// and the body part must have a collider with IsTrigger checked
	// 	(body part COULD have rigidbody, but that freezes the collider in place?)
	void OnTriggerEnter(Collider collider)
	{
		health -= 10.0f;
		Debug.Log(gameObject.name + " TRIGGER!!");
	}
}
