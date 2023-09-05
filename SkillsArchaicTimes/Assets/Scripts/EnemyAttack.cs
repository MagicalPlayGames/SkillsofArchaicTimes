using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	public GameObject target;
	
	//Hopefully used so that when they're moving or something, they don't attack
	public bool canAttack = true;
	
	// STATS remove if we add a dedicated stat script
	public float health = 100.0f;
	public float damage = 10.0f;
	public float dodge = 10.0f;
	public float miss = 10.0f;
	public float block = 10.0f;
	
	//Should actually be related to FPS, 300 is placeholder
	private int attackTimer = 300;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		//Keeping the debug rays in the same position as the actual hits
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10.0f, Color.yellow);
		
		//Attack is called periodically to avoid casting many rays when nothing happens
		if (target != null && canAttack)
		{
			attackTimer--;
			if (attackTimer <= 0)
			{
				CastAttack();
				attackTimer = 300;
			}
		}
    }
	
	// CastAttack called when timer is 0
	void CastAttack()
	{
		//Probably have to bit shift to just get a specific layer
		RaycastHit hit;
		
		//Will probably have to cast multiple times
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10.0f))
		{
			bool enemyHit = false;
			/*
			if (Blue)
			{
				if (hit.collider.gameObject.GetComponent<EnemyAttack>().Red)
				{
					enemyHit = true;
				}
			}
			if (Red)
			{
				if (hit.collider.gameObject.GetComponent<EnemyAttack>().Blue)
				{
					enemyHit = true;
				}
			}
			*/
			if (hit.collider.gameObject == target)
			{
				enemyHit = true;
			}
			if (enemyHit)
			{
				Debug.Log(gameObject.name + " hit " + hit.collider.gameObject.name);
				float randomHit = Random.Range(0.0f, 100.0f);
				
				// 0->10 is dodge
				if (randomHit <= hit.collider.gameObject.GetComponent<EnemyAttack>().dodge)
				{
					Dodge(hit.collider.gameObject);
				}
				// 10->20 is miss
				else if (randomHit <= hit.collider.gameObject.GetComponent<EnemyAttack>().dodge + miss)
				{
					Miss(hit.collider.gameObject);
				}
				// 20->30 is block
				else if (randomHit <= hit.collider.gameObject.GetComponent<EnemyAttack>().dodge + miss + block)
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
	}
	
	// Attack
	void Attack(GameObject objHit)
	{
		float dmg = damage;
		
		Debug.Log(objHit.name + " took " + dmg + " amount of damage!" + "\n"
		+ "Random point on enemy: " + RandomPoint(objHit.GetComponent<Collider>().bounds));
		
		//This will be the damage calculation, where we go into the players health and reduce it
		//objHit.GetComponent<EnemyAttack>().health -= dmg;
		
		//As for picking where the attack is coming from, I imagine it would be easier to hard code it
		//Ya know pick some point just kinda above the shoulder or whatever
		
		//I'm unsure how animating the swing would work? if we plan on doing that
		//But I'd hope that you can just start the animation at a certain point and then swing to randompoint
		//Or whatever
		//Or just leave the animation alone, just raise sword then swing down
		//And the player will see a popup where the swing is coming from to react
		
		//Otherwise, if we still need the arc, take the origin point of swing,
		//and make a line with the end point. Then integrate that line (dunno how it works in 3d tho)
		//this gives us a quadratic "arc"
		//or we could just make a function that increments by a delta until midpoint is reached, then reverse
	}
	
	// RandomPoint
	Vector3 RandomPoint(Bounds bounds)
	{
		return new Vector3(
			Random.Range(bounds.min.x, bounds.max.x),
			Random.Range(bounds.min.y, bounds.max.y),
			Random.Range(bounds.min.z, bounds.max.z)
		);
	}
	
	// Dodge
	void Dodge(GameObject objHit)
	{
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
		Debug.Log(objHit.name + " blocked the attack!");
	}
}
