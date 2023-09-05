using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine;

public class SpellHandSelect : MonoBehaviour
{

    //The input action (Grip,Trigger,Primary Axis, etc)
    [SerializeField] private InputActionAsset actionAsset;
    //Either XRI LeftHand or XRI RightHand
    [SerializeField] private string controllerName;
    //Select,Hover,or Activate action on controllerName controller
    [SerializeField] private string grip;
    public BarStatus manaBar;
    public GameObject[] spells;
    public float[] damages;
    public GameObject spellObj;
    public GameObject[] castedSpells;
    public bool spellOn;
    public bool spellCharged;
    private int castedSize;
    void Update()
    {
        if(manaBar.fillState<=0.0f)
            if (spellObj != null)
                Destroy(spellObj);
        //If the grip is onSelected (grip button pressed), and the spell is charged show makeSpell()
        if (actionAsset.FindActionMap(controllerName).FindAction(grip).IsPressed() && spellCharged && !spellOn && manaBar.fillState>0.0f)
        {
            makeSpell();
        }
        //Else if the controller is not gripped, and there has been spells casted recently,
        else if (!actionAsset.FindActionMap(controllerName).FindAction(grip).IsPressed())
        {

            if (spellObj != null)
                Destroy(spellObj);
            if (castedSize != 0)
                {
                    foreach (GameObject spell in castedSpells)
                    {
                        Destroy(spell);
                    }
                    castedSize = 0;
            }
            spellOn = false;
        }
        if (!spellCharged)
        {
            StartCoroutine(waitToCast());
        }
    }

    //Charge the spell
    IEnumerator waitToCast()
    {
        yield return new WaitForSeconds(2f);
        if(!spellOn && !spellCharged)
            spellCharged = true;
    }

    //Instantiate the runes for the spell, See SpellRune.cs and SpellWaypoint.cs
    public void makeSpell()
    {
        spellOn = true;
        spellCharged = false;
        castedSize = 0;
        foreach (GameObject spell in spells)
        {
            GameObject casted = Instantiate(spell, this.transform.position, this.transform.rotation, null);
            casted.GetComponent<SpellRune>().Hand = this.gameObject;
            castedSpells[castedSize] = casted;
            castedSize++;
        }
    }

    public void inactiveSpells(GameObject selected)
    {
        foreach (GameObject spell in castedSpells)
        {
            if(spell!=selected)
            {
                Destroy(spell);
            }
        }
    }

    public void decreaseMana(int damageIndex)
    {
        manaBar.decrease(damages[damageIndex]);
    }
}
