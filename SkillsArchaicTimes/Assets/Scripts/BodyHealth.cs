using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyHealth : MonoBehaviour
{
	public float health = 100.0f;
	public float damageMultipler;
	public HealthManagerEdit managerScript;
	public Dismemberer dismemberScript;
	public GameObject[] heldWeapons;
	public EnemyAttack2 attckScript;
	// Start is called before the first frame update
	void Start()
	{
		if (attckScript != null)
		{
			findDamageAreaLoop(attckScript.weapons);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (attckScript != null)
		{
			findDamageAreaLoop(attckScript.weapons);
		}
	}

	private void findDamageAreaLoop(GameObject[] weapons)
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			heldWeapons[i] = findDamageArea(weapons[i]);
		}
	}

	public GameObject findDamageArea(GameObject weapon)
	{
		if (weapon == null)
			return null;
		if (weapon.transform.childCount > 0)
		{
				for (int i = 0; i < weapon.transform.childCount; i++)
					if (weapon.transform.GetChild(i).GetComponent<WeaponData>() != null)
						return weapon.transform.GetChild(i).gameObject;
		}
		else if (weapon.GetComponent<WeaponData>() == null)
			return null;
		return weapon;
	}

	// OnCollisionEnter called when objects are colliding
	// Can't get this one to work, says it works "if one of the colliders also has a non-kinematic rigidbody attached"
	// but wouldn't really work properly.
	void OnTriggerEnter(Collider collision)
	{

		if ((heldWeapons[0] != null || heldWeapons[1] !=null))
		{
			for (int i = 0; i < heldWeapons.Length; i++)
			{
				if (findDamageArea(collision.gameObject) == null)
					return;
				if (heldWeapons[i] != null)
				if (findDamageArea(collision.gameObject).tag != "Weapon" || findDamageArea(collision.gameObject).transform.parent == findDamageArea(heldWeapons[i]).transform.parent || findDamageArea(collision.gameObject) == null)
					return;
			}
			Transform attacker = collision.transform;
			if(false)//May re input
			while(attacker.GetComponent<EnemyAttack2>()==null)
            {
				attacker = attacker.parent;
				if (attacker == null)
					break;
				}
			if (false)//May re input
				if (attckScript != null && attacker != null)
				attckScript.target = attacker.gameObject;

			GameObject par = collision.gameObject;
			if (collision.transform.parent != null)
				if(!collision.transform.parent.name.Contains("oldster") && !collision.transform.parent.name.Contains("andle") && collision.transform.parent.tag!="SpawnElem" && !collision.transform.parent.name.Contains("oint"))
					par = collision.transform.parent.gameObject;
			if (par.GetComponent<Rigidbody>() == null)
			{
				Debug.Log(par.name + "Doesnt have a rigidbody");
				return;
			}
			Debug.Log(par);
			//Vector3.Magnitude(par.GetComponent<Rigidbody>().velocity) * damageMultipler * 
			float damage =  findDamageArea(par.gameObject).GetComponent<WeaponData>().weaponModifier * damageMultipler;
			Debug.Log("Damage Taken: " + damage);
			loseHealth(damage);
		}
		else if (findDamageArea(collision.gameObject) != null && findDamageArea(collision.gameObject).tag == "Weapon")
		{
			GameObject par = collision.gameObject;
			while (par.transform.parent != null)
			{
				if (par.transform.parent.parent != null)
					if (par.transform.parent.parent.GetComponent<Rigidbody>() != null)
						par = par.transform.parent.parent.gameObject;

				if (par.transform.parent == null)
					break;

				if (par.transform.parent.GetComponent<Rigidbody>() != null)
					par = par.transform.parent.gameObject;

				if (par.transform.parent != null)
				{
					if (par.transform.parent.parent == null)
					{
						if (par.transform.parent.GetComponent<Rigidbody>() == null)
							break;
					}
					else if (par.transform.parent.GetComponent<Rigidbody>() == null)
						break;
				}

			}
			if(par.GetComponent<Rigidbody>()==null)
            {
				Debug.Log(par.name + "Doesnt have a rigidbody");
				return;
            }
			//Vector3.Magnitude(par.GetComponent<Rigidbody>().velocity) * damageMultipler * 
			float damage = findDamageArea(collision.gameObject).GetComponent<WeaponData>().weaponModifier* damageMultipler;
			//Debug.Log("Damage Taken: " + damage);
			loseHealth(damage);
		}
	}

	public void loseHealth(float damage)
    {

		health -= damage;
		if(managerScript!=null)
			managerScript.takeDamage(damage);
		if (health <= 0 && dismemberScript != null)
		{
			dismemberScript.Dismember();
			Destroy(this);
		}
	}
}
