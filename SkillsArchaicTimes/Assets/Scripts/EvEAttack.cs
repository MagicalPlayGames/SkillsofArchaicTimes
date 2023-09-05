using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvEAttack : MonoBehaviour
{
    /*Summary of what it should do
     *----------------------------
     * The enemy should run, then walk, then pick up a weapon
     * Next, the enemy should run, then walk, then attack the target (another enemy, or a player)
     * When attacking the enemy can either walk around the target, attack them, or block their attack
     * 
     * The head always faces the target as far as the neck can go
     * The enemy will end up having other weapons to use
     * 
     * The main problem is the animation.
     * With the animationTrigger, it is attached to the animator, attached to the AnimationController MikeTest
     * It is hard to make the enemy only side step when appropriate.
     * 
     * When an enemy is not moving their hands, they should be in Stance
     * When an enemy is moving their hands, the legs should be in some kind of stance (forward left, side right, etc)
     * There are also akward trainsitions between running, walking, and Stance for the arms
     */

    //Feetplaceent CHECK
    //LeftSide, Left Front, Right Side, Right Front

    //isClose (without shield)
    //Attack
    //Left Swing, Right Swing, Stabs, and feet placement
    //Block 
    //UpperLeft UpperRight LowerLeft LowerRight feet placement
    //Side Step
    //Left  Right

    //No weapon CHECK
    //Find a weapon
    //Search, travel, and pickup CHECK
    //No weapons found
    //??

    //With weapon
    //make a decision

    //Find extra weapon
    //See if you have time
    //travel and pickup
    //Move towards place
    //Rotate
    //Run
    //Walk
    //Turn Head

    //Rotate
    //Should rotate

    //Disable Part
    //find weapon data

    public BasicAnimationController animationTrigger;

    //Dont worry about variables until you are applying them
    public GameObject target;
    public GameObject head;
    public float speedMultiplier = 1;
    public GameObject[] weapons;
    public GameObject[] holdsters;
    private float distance;
    public float minWalkDistance;
    public float minRunDistance;
    public bool walk = false;
    public float rotateSpeed;
    public float runSpeed;
    public float walkSpeed;
    public float stepSpeed;
    public float minSideAngle;
    public float minRotateAngle;
    public bool canAttack = true;
    public bool isClose = false;
    public bool playerIsLooking = false;
    private bool sideStepRight = false;
    public bool finding = false;
    public bool cantFindWeapon = false;
    private bool sideStepLeft = false;
    private float decisionVar = 0;
    private float decisionTime = 1;
    public bool isRotating;
    private bool waiting = false;

    public bool deadArmsL;
    public bool deadArmsR;

    public float decisionCoolDown = 0;

    public bool debug;




    void Start()
    {
        if (holdsters[0].transform.childCount > 0)
        {
            deadArmsR = false;
        }
        else
        {
            deadArmsR = true;
        }

        if (holdsters[1].transform.childCount > 0)
        {
            deadArmsL = false;
        }
        else
        {
            deadArmsL = true;
        }

        animationTrigger.setBool("DeadRArm", deadArmsR);
        animationTrigger.setBool("DeadLArm", deadArmsL);

        animationTrigger.trigger("LEmpty", false);
        animationTrigger.trigger("REmpty", false);
        animationTrigger.trigger("LegsEmpty", false);
        if (target == null)
            findTarget();

        distance = Vector3.Distance(this.gameObject.transform.position, target.transform.position);
    }

    private void walking()
    {
        animationTrigger.trigger("Walk", false);
        animationTrigger.deactivate("Run");
        animationTrigger.deactivate("LSideStep");
        animationTrigger.deactivate("RSideStep");
        transform.position += transform.forward * walkSpeed * speedMultiplier * Time.deltaTime;
    }

    private void running()
    {
        animationTrigger.trigger("Run", false);
        animationTrigger.deactivate("Walk");
        animationTrigger.deactivate("LSideStep");
        animationTrigger.deactivate("RSideStep");
        transform.position += transform.forward * runSpeed * speedMultiplier * Time.deltaTime;
    }
    // Update is called once per frame
    void Update()
    {
        if (!waiting)
        {
            //DISABLED FOR NOW
            if (target != null)//If the enemy has a weapon, and needs to face the player
            {
                distance = Vector3.Distance(this.gameObject.transform.position, target.transform.position);
                if (debug)
                    Debug.Log("Face Player");
                lookAtPlayer();
                if (distance < minWalkDistance + minWalkDistance / 2.0f && isClose && distance > minWalkDistance)
                    isClose = false;
                else if (distance < minWalkDistance && !isClose)
                    isClose = true;
            }
            else if (target == null)//Finds the player
            {
                findTarget();
            }
            if (distance > minWalkDistance && !(sideStepLeft || sideStepRight) && !isClose)//Moves to the player if not side stepping, and is not close
            {
                    if (debug)
                        Debug.Log("Going to Player");
                    moveToPlayer();
            }
            else if (!(sideStepLeft || sideStepRight) && isClose)//If it is in close range, call closeRange()
            {
                animationTrigger.trigger("DoneStepping", false);
                animationTrigger.trigger("StopMoving", false);
                closeRange();
            }
        }
    }

    //Finds the player target
    void findTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = 100;
        int index = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != this.gameObject)
            {
                float distance = Vector3.Distance(this.gameObject.transform.position, targets[i].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                }
            }
        }
        target = targets[index];
    }

    //How to move towards the player
    //(1) Run
    //(2) Walk
    //(3) Rotate
    void moveToPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit);
        if (hit.collider != null)
        {
            if (animationTrigger.m_Animator.GetBool("ArmsFinished"))
            {
                if (distance > minRunDistance + minRunDistance / 4.0f && walk)//If the enemy is around the minRunDistance, but is currently walking
                    walk = false;

                if (distance > minRunDistance && !walk)//(1)
                {
                    if (hit.collider.gameObject != target && shouldRotate(target))//(3) rotates without animation. They are still running
                    {
                        rotate(target, false, rotateSpeed);
                    }
                    isRotating = false;
                    animationTrigger.trigger("DoneStepping", false);
                    running();
                }
                //(2)
                else if ((distance < minRunDistance && distance > minWalkDistance) || walk)//Needs Blend tree that blends walking and running by distance
                {
                    if (animationTrigger.m_Animator.GetBool("ArmsFinished"))
                    {
                        walk = true;
                        if (hit.collider.gameObject != target && shouldRotate(target))//(3) rotates without animation. They are still walking
                        {
                            rotate(target, false, rotateSpeed);
                        }
                        animationTrigger.trigger("DoneStepping", false);
                        isRotating = false;
                        walking();
                    }
                }
                else if (hit.collider.gameObject != target && shouldRotate(target))//(3)
                {
                    if (animationTrigger.m_Animator.GetBool("ArmsFinished"))
                        rotate(target, true, stepSpeed);
                    animationTrigger.deactivate("Walk");
                    animationTrigger.deactivate("Run");
                }
            }
        }
        else//(3)
        {
            animationTrigger.deactivate("DoneStepping");
            if (animationTrigger.m_Animator.GetBool("ArmsFinished"))
                rotate(target, true, stepSpeed);
        }
    }

    //Decides if the target is far enough to rotate towards them
    bool shouldRotate(GameObject target)
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0;
        bool should = Vector3.Angle(transform.forward, targetDirection) > minRotateAngle;
        return should;
    }

    //Rotates around
    void rotate(GameObject targ, bool toSide, float speed)
    {
        Vector3 targetDirection = targ.transform.position - transform.position;
        targetDirection.y = 0;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, speed * speedMultiplier * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection, new Vector3(0, 20, 0));
        if (toSide)//If you want to animate the rotation
        {
            isRotating = true;
            Vector3 rightDir = Vector3.RotateTowards(transform.right, targetDirection, speed * speedMultiplier * Time.deltaTime, 0.0f);
            Vector3 leftDir = Vector3.RotateTowards(-transform.right, targetDirection, speed * speedMultiplier * Time.deltaTime, 0.0f);
            if (Vector3.Angle(rightDir, targetDirection) > Vector3.Angle(leftDir, targetDirection))//If enemy is on the right
            {
                animationTrigger.trigger("RSideStep", false);
            }
            else//If enemy is on the left
            {
                animationTrigger.trigger("LSideStep", false);
            }
        }
    }

    //Decisions to make when in close range.
    //(1) faces the enemy
    //(2) Attack(2.1),Block(2.2), or step to the side(2.3)
    void closeRange()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit);
        if (hit.collider == null)//(1)
        {
            if (shouldRotate(target))
            {
                animationTrigger.deactivate("DoneStepping");
                if (animationTrigger.m_Animator.GetBool("ArmsFinished"))
                    rotate(target, true, stepSpeed);
            }
            else
            {
                if (decisionTime == decisionVar)//Makes sure a decision wasn't just made
                    return;
                decisionTime = decisionVar;
                    StartCoroutine(decisionWait(decisionCoolDown / speedMultiplier));
                    float randomHit = Random.Range(0.0f, 30.0f);
                    if (randomHit <= 13)//(2.2)
                    {
                        block();
                    }
                    else//(2.1)
                    {
                        attack();
                    }
            }
        }
        else if (hit.collider.gameObject != target && shouldRotate(target))//(1)
        {
            animationTrigger.deactivate("DoneStepping");
            if (animationTrigger.m_Animator.GetBool("ArmsFinished"))
                rotate(target, true, stepSpeed);
        }
        else//(2)
        {
            if (decisionTime == decisionVar)//Makes sure a decision wasn't just made
                return;
            decisionTime = decisionVar;
                    StartCoroutine(decisionWait(decisionCoolDown / speedMultiplier));
                    float randomHit = Random.Range(0.0f, 30.0f);
                    if (randomHit <= 13)//(2.2)
                    {
                        block();
                    }
                    else//(2.1)
                    {
                        attack();
                    }
        }
    }

    //randomly blocks
    //DeadArm shows that the arm is not attached
    void block()
    {
        if (debug)
            Debug.Log("Block");
        float randomHit = Random.Range(0.0f, 40.0f);
        if (randomHit < 20 && !(animationTrigger.m_Animator.GetBool("DeadLArm")))
        {
            if (randomHit < 10)
                animationTrigger.trigger("LMBlock", true);
            else if (randomHit < 20)
                animationTrigger.trigger("LHBlock", true);

            animationTrigger.trigger("REmpty", true);
        }
        else if (!(animationTrigger.m_Animator.GetBool("DeadRArm")))
        {
            if (randomHit < 30)
                animationTrigger.trigger("RMBlock", true);
            else
                animationTrigger.trigger("RHBlock", true);

            animationTrigger.trigger("LEmpty", true);
        }
    }
    //Always makes the head face the target
    void lookAtPlayer()
    {

        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0;
        float sideAngle = Vector3.Angle(transform.right, targetDirection);
        animationTrigger.m_Animator.SetFloat("Movement", ((sideAngle - 60) / 60));
    }

    //Randomly attacks. Focuses on arms
    void attack()
    {
        if (debug)
            Debug.Log("Attack");
        float randomHit = Random.Range(0.0f, 40.0f);
        if (randomHit < 20 && !(animationTrigger.m_Animator.GetBool("DeadLArm")))
        {
            if (randomHit < 10)
                animationTrigger.trigger("LSwing", true);
            else
                animationTrigger.trigger("LStab", true);

            animationTrigger.trigger("REmpty", true);
        }
        else if (!(animationTrigger.m_Animator.GetBool("DeadRArm")))
        {
            if (randomHit < 30)
                animationTrigger.trigger("RSwing", true);
            else
                animationTrigger.trigger("RStab", true);
            animationTrigger.trigger("LEmpty", true);
        }
    }
    //This rotates the enemy to alwasy face the target, and move left or right depending on which direction they are moving to
    void sideStep()
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * speedMultiplier * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection, new Vector3(0, 1, 0));
        if (sideStepLeft)
        {
            animationTrigger.trigger("LSideStep", false);
            transform.position -= transform.right * stepSpeed * speedMultiplier * Time.deltaTime;
        }
        else
        {
            animationTrigger.trigger("RSideStep", false);
            transform.position += transform.right * stepSpeed * speedMultiplier * Time.deltaTime;
        }
    }
    //This is for how long the enemy steps to the side when that decision is made in closeRange()
    IEnumerator stepTime()
    {
        float randomVar = Random.Range(0.0f, 5.0f);
        yield return new WaitForSeconds(randomVar);
        sideStepLeft = false;
        sideStepRight = false;
        if (debug)
            Debug.Log("StopStepping");
        animationTrigger.trigger("DoneStepping", false);

        decisionVar = Random.Range(0.0f, 100.0f);
    }
    //This makes the enemy wait to make the next move
    IEnumerator decisionWait(float waitVar)
    {
        yield return new WaitForSeconds(waitVar);
        animationTrigger.trigger("StopMoving", false);
        decisionVar = Random.Range(0.0f, 100.0f);
    }
}
