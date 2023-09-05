using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponData : MonoBehaviour
{
    public float weaponModifier;
    public bool grabbed = false;
    private float startModifier = 0;

    public float hitSomething = 0;
    // Start is called before the first frame update
    private void Start()
    {
        if(startModifier==0)
            startModifier = weaponModifier;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Shield")
        {
            if(this.gameObject.tag!="Shield")
            {
                weaponModifier = 0;
                StartCoroutine(shieldHit());
            }

        }
        hitSomething = 1;
    }

    IEnumerator waitForHit()
    {
        yield return new WaitForSeconds(0.1f);
        hitSomething = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hitSomething = 1;
            StartCoroutine(waitForHit());
            if (this.gameObject.transform.parent != null)
                if (this.gameObject.transform.parent.parent != null)
                    if (this.gameObject.transform.parent.GetComponent<XRGrabInteractable>() != null)
                        if (!this.gameObject.transform.parent.GetComponent<XRGrabInteractable>().isSelected)
                        {
                            other.gameObject.GetComponent<PlayerHealth>().decrease(weaponModifier);
                            return;
                        }

            if (this.gameObject.transform.GetComponent<XRGrabInteractable>() != null)
                if (this.gameObject.transform.parent != null)
                    if (!this.gameObject.transform.GetComponent<XRGrabInteractable>().isSelected)
                    other.gameObject.GetComponent<PlayerHealth>().decrease(weaponModifier);
        }
    }

    IEnumerator shieldHit()
    {
        yield return new WaitForSeconds(0.05f);
        if (weaponModifier == 0)
        {
            yield return new WaitForSeconds(0.5f);
            weaponModifier = startModifier;
        }
    }
    public void setGrabbed(bool x)
    {
        grabbed = x;
        XRGrabInteractable obj;
        if(this.gameObject.transform.parent!=null)
            if (this.gameObject.transform.parent.GetComponent<XRGrabInteractable>() != null)
                obj = this.gameObject.transform.parent.GetComponent<XRGrabInteractable>();
            else
            obj = this.gameObject.transform.GetComponent<XRGrabInteractable>();
        else
            obj = this.gameObject.transform.GetComponent<XRGrabInteractable>();

        if (x)
        {
            weaponModifier = startModifier;
        }
        else
        {
            weaponModifier = 0;
        }
        if (obj != null)
            obj.enabled = !x;
    }

    public void pickUp(bool x)
    {
        if(startModifier!=0)
            weaponModifier = startModifier;
        grabbed = x;
    }

    public void disableWeapon()
    {
        if(startModifier==0)
            startModifier = weaponModifier;
        weaponModifier = 0;
    }
}
