using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Nathan Heslep
//Edited by Michael Alcala
public class EnemyAttackEdits : MonoBehaviour
{
	/*public GameObject target;

	//Hopefully used so that when they're moving or something, they don't attack
	public bool canAttack = true;
	public bool targetPlayer;

	// STATS remove if we add a dedicated stat script

	//I want to make a debug menu that lets us modify all stats!!! even the random variables MA

	public BasicAnimationController animationTrigger;

	//Should actually be related to FPS, 300 is placeholder
	//I included a coroutine instead, it goes off of seconds. MA
	// I'm still keeping the variable
	//made public for testing
	public int attackTimer = 3;
	// Start is called before the first frame update
	void Start()
	{
		if (targetPlayer)
		{
			//This will make it to where target is found right on spawn (MA)
			target = GameObject.FindGameObjectWithTag("Player");
		}
	}

	// Update is called once per frame
	void Update()
	{
		//Keeping the debug rays in the same position as the actual hits
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10.0f, Color.yellow);

		//Attack is called periodically to avoid casting many rays when nothing happens
		if (canAttack && !animationTrigger.triggered)
		{
			CastAttack();
			StartCoroutine(waitToCharge());
		}
	}

	IEnumerator waitToCharge()
	{
		canAttack = false;
		yield return new WaitForSeconds(attackTimer);
		canAttack = true;
	}

	// CastAttack called when timer is 0
	void CastAttack()
	{
		//Probably have to bit shift to just get a specific layer
		RaycastHit hit;

		//Will probably have to cast multiple times
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10.0f))
		{
			float randomHit = Random.Range(0.0f, 100.0f);

			// 0->10 is dodge
			if (randomHit <= 30)
			{
				Block(hit.collider.gameObject);
			}
			// 30->100 is hit
			else
			{
				Attack(hit.collider.gameObject);
			}
		}
	}

	// Attack
	void Attack(GameObject objHit)
	{
		float randomHit = Random.Range(0.0f, 40.0f);
		if (randomHit < 20)
		{
			if (randomHit < 10)
				animationTrigger.trigger("LSwing");
			else
				animationTrigger.trigger("LStab");

			animationTrigger.trigger("REmpty");
		}
		else
		{
			if (randomHit < 30)
				animationTrigger.trigger("RSwing");
			else
				animationTrigger.trigger("RStab");
			animationTrigger.trigger("LEmpty");
		}
		randomHit = Random.Range(0.0f, 40.0f);
		if (randomHit < 10)
			animationTrigger.trigger("RSStance");
		else if (randomHit < 20)
			animationTrigger.trigger("LSStance");
		else if (randomHit < 30)
			animationTrigger.trigger("RFStance");
		else
			animationTrigger.trigger("LFStance");
	}

	// Dodge
	void Dodge(GameObject objHit)
	{
		float randomHit = Random.Range(0.0f, 100.0f);
		Debug.Log(objHit.name + " dodged!");
	}

	// Miss
	void Miss(GameObject objHit)
	{
		Debug.Log(gameObject.name + " missed their attack!");
	}

	// Block
	void Block(GameObject objHit)
	{
		float randomHit = Random.Range(0.0f, 40.0f);
		if (randomHit < 20)
		{
			if (randomHit < 10)
				animationTrigger.trigger("LMBlock");
			else if (randomHit < 20)
				animationTrigger.trigger("LHBlock");

			animationTrigger.trigger("REmpty");
		}
		else
		{
			if (randomHit < 30)
				animationTrigger.trigger("RMBlock");
			else
				animationTrigger.trigger("RHBlock");

			animationTrigger.trigger("LEmpty");
		}

		randomHit = Random.Range(0.0f, 20.0f);
		if (randomHit < 10)
			animationTrigger.trigger("LSStance");
		else
			animationTrigger.trigger("RSStance");
	}*/
}
