using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Dismemberer : MonoBehaviour
{
	//The next child main part connteded to the joint
	public GameObject bodyPart;
	//This joint object
	public GameObject jointPart;
	//The child joint object
	public Dismemberer childScript;
	public EnemyAttack2 attackScript;
	//Renderer for animations
	private SkinnedMeshRenderer skinnedRender;
	//Non-animated Renderers
	private MeshRenderer meshRender;
	private MeshFilter meshFilter;


	/* Dismemberer Script
	 * Used to dismember pieces of the enemy when joints are collided with.
	 */

	private void Start()
	{
		if (this.GetComponent<Rigidbody>() != null)
			this.GetComponent<Rigidbody>().isKinematic = true;

		skinnedRender = bodyPart.GetComponent<SkinnedMeshRenderer>();
	}

	//When hit with a weapon

	//changes the parent, and kinematics of bodyPart, calling JointHit(), and destroying the object
	public void Dismember()
	{
		int i = 0;
		while (bodyPart.transform.childCount > 0)
		{
			if (i >= bodyPart.transform.childCount)
				break;
			if (!bodyPart.transform.GetChild(i).name.Contains("andle"))
			{
				bodyPart.transform.GetChild(i).parent = null;
			}
			i++;
		}
		bodyPart.transform.parent = null;
		bodyPart.GetComponent<Rigidbody>().isKinematic = false;
		bodyPart.GetComponent<XRGrabInteractable>().enabled = true;
		bodyPart.GetComponent<Collider>().isTrigger = false;
		GameObject jointHolder = jointPart;
		jointPart = null;
		JointHit();
		if (jointHolder != null)
		{
			Destroy(jointHolder);
		}

	}

	//Called by Dismember(), this changes the hierachy of items from the prefab out, and calls switchRender()
	public void JointHit()
	{
		if (jointPart != null)
			bodyPart.transform.parent = jointPart.transform;
		switchRender();
		if (childScript != null)
		{
			childScript.JointHit();
			if (childScript.jointPart != null)
				childScript.jointPart.transform.parent = bodyPart.transform;
		}
		bodyPart.transform.tag = "Weapon";
		bodyPart.layer = 6;//Weapon Layer
		Destroy(this);
	}



	//Changes the applied renderers from the animated mesh to the static mesh
	private void switchRender()
	{
		if (bodyPart.name.Contains("Hand") || bodyPart.name.Contains("Leg") || bodyPart.name.Contains("Foot"))
		{
			if (attackScript != null)
			{
				attackScript.DisablePart(bodyPart.name);
			}
		}
		skinnedRender.enabled = false;
		if (bodyPart.GetComponent<MeshRenderer>() == null)
		{
			meshRender = bodyPart.AddComponent<MeshRenderer>();
			meshFilter = bodyPart.AddComponent<MeshFilter>();
		}
		else
		{
			meshRender = bodyPart.GetComponent<MeshRenderer>();
			meshFilter = bodyPart.GetComponent<MeshFilter>();
		}
		if (jointPart != null)
		{
			if (jointPart.GetComponent<SkinnedMeshRenderer>() != null)
			{
				SkinnedMeshRenderer sR = jointPart.GetComponent<SkinnedMeshRenderer>();
				MeshRenderer mR = jointPart.AddComponent<MeshRenderer>();
				MeshFilter mF = jointPart.AddComponent<MeshFilter>();
				sR.enabled = false;
				if(mF!=null)
					mF.mesh = sR.sharedMesh;
				if(mR!=null)
					mR.materials = sR.materials;
			}
		}
		if(bodyPart.GetComponent<WeaponData>()==null)
        {
			bodyPart.AddComponent<WeaponData>().weaponModifier = 0.4f;
			bodyPart.GetComponent<WeaponData>().disableWeapon();
		}
		meshFilter.mesh = skinnedRender.sharedMesh;
		meshRender.materials = skinnedRender.materials;
	}
}
