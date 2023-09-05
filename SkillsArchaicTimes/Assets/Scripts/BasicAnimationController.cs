using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine;

public class BasicAnimationController : MonoBehaviour
{
    public Animator m_Animator;
    public bool triggered = false;
    public string[] triggerHere;
    public float speedMultiplier = 1.0f;
    //LSStance
    //LFStance
    //R...
    //LSwing
    //LStab
    //LHB
    //LLB
    //R...
    //Idle
    // Start is called before the first frame update
    public void Start()
    {
        m_Animator.SetFloat("SpeedMultipler", speedMultiplier);
    }
    private void Update()
    {
        if (triggered)
            StartCoroutine(waitForIt());
    }

    IEnumerator waitForIt()
    {
        bool[] done= new bool[2];
        done[0] = false;
        done[1] = false;
        triggered = false;
        yield return new WaitForSeconds(1);
        for (int i = 1; i < 3; i++)
        {
            if (m_Animator.GetCurrentAnimatorClipInfoCount(i) == 1)
            {
                for(int j = 0;j<triggerHere.Length;j++)
                {
                    if (m_Animator.GetCurrentAnimatorClipInfo(i)[0].clip.name == triggerHere[j])
                    {
                        done[i - 1] = true;
                        break;
                    }
                }
            }
        }
        if (done[0]&&done[1])
        {
            m_Animator.SetBool("ArmsFinished", true);
        }
        else
        {
            StartCoroutine(waitForIt());
        }
    }
    public void trigger(string action,bool syncronize)
    {
        m_Animator.SetTrigger(action);
        if (syncronize)
        {
            m_Animator.SetBool("ArmsFinished", false);
            triggered = true;
        }
    }
    public void armAttack()
    {
        m_Animator.SetBool("ArmsFinished", false);
        triggered = true;
    }

    public void setInt(string action, int num)
    {
        m_Animator.SetInteger(action, num);
    }
    public int getInt(string action)
    {
        return m_Animator.GetInteger(action);
    }

    public void setBool(string action,bool set)
    {
        m_Animator.SetBool(action, set);
    }

    public bool getBool(string action)
    {
        return m_Animator.GetBool(action);
    }


    public void deactivate(string action)
    {
        m_Animator.ResetTrigger(action);
    }
}
