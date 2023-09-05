using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpellRune : MonoBehaviour
{ 
    public int spellNum;
    //Prefab
    public GameObject spellAction;
    //set by SpellHandSelect to controllerName
    public GameObject Hand;

    //Spell is active and hands control over to the SpellWaypoint
    //When drawn = true, then the fireball is created
    void Update()
    {
        if (Hand != null)
        {
            Hand.GetComponent<SpellHandSelect>().spellOn = true;
        }
    }

    public void draw()
    {
        GameObject spellObj = Instantiate(spellAction, Hand.transform.position, Hand.transform.rotation, Hand.transform);
        if (spellNum == 0)
        {
            XRInteractionManager interactionManager = Hand.GetComponent<XRDirectInteractor>().interactionManager;
            interactionManager.ForceSelect(Hand.GetComponent<XRDirectInteractor>(), spellObj.GetComponent<XRGrabInteractable>());
            spellObj.GetComponent<WeaponData>().grabbed = true;
            Hand.GetComponent<SpellHandSelect>().decreaseMana(0);
        }
        else if(spellNum==1)
        {
            Hand.GetComponent<SpellHandSelect>().spellObj = spellObj;
            spellObj.GetComponent<IceBeam>().hand = Hand.GetComponent<SpellHandSelect>();
        }
        else if(spellNum==2)
        {
            Hand.GetComponent<SpellHandSelect>().spellObj = spellObj;
            spellObj.GetComponent<Healing>().hand = Hand.GetComponent<SpellHandSelect>();
        }
        Destroy(this.gameObject);
    }

    public void DestroyOthers()
    {
        Hand.GetComponent<SpellHandSelect>().inactiveSpells(this.gameObject);
    }

    public void turnOn()
    {
        for(int i =0;i<this.transform.childCount;i++)
        {
            if(!this.transform.GetChild(i).gameObject.activeSelf)
            {
                this.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
