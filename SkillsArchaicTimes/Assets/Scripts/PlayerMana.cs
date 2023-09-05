using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    public BarStatus manaBar;
    public SpellHandSelect[] spellHands;
    public float chargeRate;
    private void Update()
    {
        if (manaBar.fillState < 1.0f && (!spellHands[0].spellOn && !spellHands[1].spellOn))
            manaBar.decrease(-chargeRate);
    }
    public void decrease(float amount)
    {
        manaBar.decrease(amount);
    }
}
