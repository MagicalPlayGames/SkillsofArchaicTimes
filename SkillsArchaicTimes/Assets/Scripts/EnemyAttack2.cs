using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack2 : MonoBehaviour
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
    private bool isRotating;
    private bool waiting = false;
    private bool rightHold = false;

    public bool deadArmsL;
    public bool deadArmsR;
    public bool noArmL;
    public bool noArmR;
    public bool deadLegL;
    public bool deadLegR;
    public bool aDeadFoot;

    public float decisionCoolDown = 0;

    public bool debug;




    void Start()
    {
        if (holdsters[0].transform.childCount > 0)
        {
            deadArmsR = false;
            weapons[0] = findWeaponData(holdsters[0].transform.GetChild(0).gameObject).gameObject;
        }
        else
        {
            deadArmsR = true;
        }

        if (holdsters[1].transform.childCount > 0)
        {
            deadArmsL = false;
            weapons[1] = findWeaponData(holdsters[1].transform.GetChild(0).gameObject).gameObject;
        }
        else
        {
            deadArmsL = true;
        }

        for(int i =0;i<weapons.Length;i++)
        {
            if (weapons[i] != null)
                if(findWeaponData(weapons[i])!=null)
                findWeaponData(weapons[i]).setGrabbed(true);
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

    private void stance(float angle)
    {
        animationTrigger.trigger("DoneStepping", false);
        if (angle < -minSideAngle)//If target is on the left
        {
            animationTrigger.trigger("LSStance", false);
        }
        else if (angle > minSideAngle)//If target is on the right
        {
            animationTrigger.trigger("RSStance", false);
        }
        else//If target is infront of enemy
        {
            float randomStance = Random.Range(0.0f, 20.0f);
            if (randomStance < 10)
                animationTrigger.trigger("RFStance", false);
            else
                animationTrigger.trigger("LFStance", false);
        }
    }

    private bool isExluded(GameObject item, GameObject[] list)
    {
        foreach(GameObject choice in list)
        {
            if (choice == item)
                return true;
        }

        return false;
    }

    private GameObject findItem(GameObject[] options, bool findDamageItems, GameObject[] exclusions=null)
    {
        int index = -1;
        float minDamage = 0;
        float minItemDistance = 1000;
        for (int i = 0; i < options.Length; i++)
        {
            GameObject selectedItem = options[i];
            if (selectedItem != null)
            {
                if (!isExluded(selectedItem, exclusions))
                {
                    if (findDamageItems && !findWeaponData(selectedItem).GetComponent<WeaponData>().grabbed)
                    {
                        float distance = Vector3.Distance(this.gameObject.transform.position, options[i].transform.position);

                        if (minItemDistance > distance)
                        {
                            if (findDamageItems)
                            {
                                if (minDamage < findWeaponData(selectedItem).weaponModifier)
                                {
                                    minItemDistance = distance / 2.0f;
                                    index = i;
                                    minDamage = findWeaponData(selectedItem).weaponModifier;
                                }
                            }
                            else
                            {
                                minItemDistance = distance;
                                index = i;
                            }
                        }

                    }
                    else if(!findDamageItems)
                    {

                        float distance = Vector3.Distance(this.gameObject.transform.position, options[i].transform.position);

                        if (minItemDistance > distance)
                        {
                            if (findDamageItems)
                            {
                                if (minDamage < findWeaponData(selectedItem).weaponModifier)
                                {
                                    minItemDistance = distance / 2.0f;
                                    index = i;
                                    minDamage = findWeaponData(selectedItem).weaponModifier;
                                }
                            }
                            else
                            {
                                minItemDistance = distance;
                                index = i;
                            }
                        }
                    }
                }
            }

        }
        if (index == -1)
            return null;
        else if (findDamageItems)
            return findWeaponData(options[index]).gameObject;
        else
            return options[index];
    }
    //Selects the closest weapon, that doesn't have any parent

    private GameObject findShield()
    {
        GameObject[] shields = GameObject.FindGameObjectsWithTag("Shield");
        GameObject selectedItem = findItem(shields,false,weapons);
        return selectedItem;
    }
    private GameObject findAttackWeapon()
    {
        GameObject[] weapon = GameObject.FindGameObjectsWithTag("Weapon");
        GameObject selectedItem = findItem(weapon, false,weapons);
        return selectedItem;
    }
    void findAWeapon()
    {
        GameObject selectedItem;
        if (weapons[0] != null)
        {
            selectedItem = findShield();

            if (selectedItem != null)
            {
                if (weapons[0] != null)
                    weapons[1] = weapons[0];
                weapons[0] = selectedItem;
                finding = true;
                return;
            }

        }

        selectedItem = findAttackWeapon();
        if (selectedItem != null)
        {
            if (weapons[0] != null)
                weapons[1] = weapons[0];

            weapons[0] = selectedItem;
            finding = true;
        }
        else
            cantFindWeapon = true;
    }

    private void setParet(GameObject child, GameObject parent)
    {
        if (child == null)
            return;
        if (parent != null)
        child.transform.parent = parent.transform;
        
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
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

    IEnumerator pickup()
    {
        waiting = true;
        yield return new WaitForSeconds(0.75f);
        if(weapons[0]==null)
        {
            finding = false;
            waiting = false;
            if (debug)
                Debug.Log("Weapon Taken");
        }
        else if (weapons[0].GetComponent<WeaponData>().grabbed)
        {
            finding = false;
            waiting = false;
            weapons[0] = null;
            if (debug)
                Debug.Log("Weapon Taken");
        }
        else
        {
            GameObject handle = null;
            if (weapons[0].transform.childCount > 0)
                for (int i = 0; i < weapons[0].transform.childCount; i++)
                {
                    if (weapons[0].transform.GetChild(i).name.Contains("andle"))
                    {
                        handle = weapons[0].transform.GetChild(i).gameObject;
                        break;
                    }
                }
            if (handle != null)
            {
                if (rightHold)
                {
                    handle.transform.parent = null;
                    weapons[0].transform.parent = handle.transform;
                    setParet(handle, holdsters[0]);
                }
                else
                {

                    handle.transform.parent = null;
                    weapons[0].transform.parent = handle.transform;
                    setParet(handle, holdsters[1]);
                }
            }
            else if (weapons[0].transform.parent != null)
            {
                if (rightHold)
                    setParet(weapons[0].transform.parent.gameObject, holdsters[0]);
                else
                    setParet(weapons[0].transform.parent.gameObject, holdsters[1]);
            }
            else
            {
                if (rightHold)
                    setParet(weapons[0], holdsters[0]);
                else
                    setParet(weapons[0], holdsters[1]);
            }
        }
        yield return new WaitForSeconds(0.75f);
        if (weapons[0] != null)
        {
            weapons[0].GetComponent<WeaponData>().setGrabbed(true);
            waiting = false;
            finding = false;
        }
    }

    //findAWeapon() selects a weapon to go get, travelToWeapon() travels to and picks up the weapon
    void travelToWeapon()
    {
        GameObject selectedWeapon = weapons[0];
        if(selectedWeapon ==null)
        {
            finding = false;
            if (debug)
                Debug.Log("Weapon Deleted");
            return;
        }
        if (selectedWeapon.GetComponent<WeaponData>().grabbed)
        {
            finding = false;
            weapons[0] = selectedWeapon = null;
            if (debug)
                Debug.Log("Weapon Taken");
            return;
        }
        if (shouldRotate(selectedWeapon))//If it needs to rotate to the weapon, rotate
        {
            rotate(selectedWeapon,true,rotateSpeed);
        }
            float weaponDistance = Vector3.Distance(transform.position, weapons[0].transform.position);
            if (weaponDistance < 6)//Walk
            {
                walking();
            }
            else//Run
            {
                running();
            }
            if (weaponDistance < 4)//Pick up
            {
                float randomVar = Random.Range(0.0f, 1.0f);
            if ((randomVar < 0.5f || noArmL) && !noArmR && holdsters[0].transform.childCount == 0)
            {
                //pick up with left
                deadArmsR = false;
                animationTrigger.setBool("DeadRArm", false);
                animationTrigger.trigger("RPickup", true);
                if (weapons[0].transform.name.Contains("hield"))
                    animationTrigger.setBool("ShieldR", true);
                rightHold = true;
            }
            else if (!noArmL && holdsters[1].transform.childCount == 0)
            {
                //pick up with right
                deadArmsL = false;
                animationTrigger.setBool("DeadLArm", false);
                animationTrigger.trigger("LPickup", true);
                if (weapons[0].transform.name.Contains("hield"))
                    animationTrigger.setBool("ShieldL", true);
                rightHold = false;
            }
            else
            {
                finding = false;
                cantFindWeapon = true;
                return;
            }
                if (weapons[0].transform.childCount == 0 && weapons[0].transform.parent != null)
            {
                weapons[0].transform.parent.GetComponent<Rigidbody>().useGravity = false;
                weapons[0].transform.parent.GetComponent<Collider>().isTrigger = true;
                weapons[0].transform.parent.GetComponent<Rigidbody>().isKinematic = true;
                weapons[0].transform.parent.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
                }
                else
            {
                weapons[0].GetComponent<Rigidbody>().useGravity = false;
                weapons[0].GetComponent<Collider>().isTrigger = true;
                weapons[0].GetComponent<Rigidbody>().isKinematic = true;
                weapons[0].GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
                }
                finding = false;//weapon is found
                StartCoroutine(pickup());
        }
    }

    private void letGo(GameObject item)
    {
        if(debug)
        Debug.Log("Letting go of " + item);
        if (item == null)
            return;
        if (item.GetComponent<Rigidbody>() == null)
            item = transform.GetComponentInChildren<Rigidbody>().gameObject;
        item.GetComponent<Collider>().isTrigger = false;
        item.GetComponent<Rigidbody>().isKinematic = false;
        item.GetComponent<Rigidbody>().useGravity = true;
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Extrapolate;
        item.transform.parent = null;
        if (findWeaponData(item) != null)
        {
            findWeaponData(item).setGrabbed(false);
            for(int i =0;i<weapons.Length;i++)
            {
                if (weapons[i] != null)
                    if (findWeaponData(item).gameObject == weapons[i].gameObject)
                    {
                        weapons[i] = null;
                        break;
                    }   
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!waiting)
        {
            //DISABLED FOR NOW
            if (weapons[0] == null && weapons[1] == null && !(noArmL && noArmR) && !finding && !cantFindWeapon)//If the enemy does not have a weapon
            {
                if (debug)
                    Debug.Log("Run to Weapon");
                findAWeapon();
            }
            if (target != null && !finding)//If the enemy has a weapon, and needs to face the player
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
            if (finding && !(noArmL && noArmR) && !cantFindWeapon)//Go get the weapon selected by findAWeapon()
            {
                if ((deadArmsL || deadArmsR) && !(noArmL && noArmR))
                    travelToWeapon();
                else
                {
                    cantFindWeapon = true;
                    finding = false;
                }
                if (target != null)
                    distance = Vector3.Distance(this.gameObject.transform.position, target.transform.position);

                cantFindWeapon = false;
            }
            else if (target == null)//Finds the player
            {
                findTarget();
            }
            else if (distance > minWalkDistance && !(sideStepLeft || sideStepRight) && !isClose)//Moves to the player if not side stepping, and is not close
            {
                if (distance > minRunDistance * 2.0f && (weapons[1] == null || weapons[0] == null) && !cantFindWeapon)
                {
                    findAWeapon();
                }
                else
                {
                    if (debug)
                        Debug.Log("Going to Player");
                    moveToPlayer();
                }
            }
            else if (sideStepLeft || sideStepRight)//Steps to the side if it is decided in closeRange()
            {
                if (debug)
                    Debug.Log("sideStepping");
                sideStep();
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
        target = GameObject.FindGameObjectWithTag("Player");
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
                    rotate(target,true, stepSpeed);
            }
        }
        else if (hit.collider.gameObject != target && shouldRotate(target))//(1)
        {
            animationTrigger.deactivate("DoneStepping");
            if (animationTrigger.m_Animator.GetBool("ArmsFinished"))
                rotate(target,true, stepSpeed);
        }
        else//(2)
        {
            if (decisionTime == decisionVar)//Makes sure a decision wasn't just made
                return;
            decisionTime = decisionVar;
            if (playerIsLooking && decisionVar < 10)//playerLook will be implemented later
            {
                StartCoroutine(stepTime());
                if (!sideStepLeft && !sideStepRight)//(2.3)
                {
                    float randomStep = Random.Range(0.0f, 1f);
                    if (randomStep < .5)
                    {
                        animationTrigger.trigger("LSideStep", false);
                        sideStepLeft = true;
                    }
                    else
                    {
                        animationTrigger.trigger("RSideStep", false);
                        sideStepRight = true;
                    }
                }
            }
            else
            {
                if (!(animationTrigger.m_Animator.GetBool("DeadRArm") && animationTrigger.m_Animator.GetBool("DeadLArm")))//This selects the stance to take
                {
                    StartCoroutine(decisionWait(decisionCoolDown / speedMultiplier));
                    float randomHit = Random.Range(0.0f, 30.0f);

                    Vector3 targetDirection = target.transform.position - transform.position;
                    targetDirection.y = 0;
                    Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * speedMultiplier * Time.deltaTime, 0.0f);
                    float sideAngle = Quaternion.Angle(Quaternion.Euler(newDirection), Quaternion.Euler(targetDirection));
                    stance(sideAngle);
                    if (randomHit <= 13 && !(animationTrigger.m_Animator.GetBool("ShieldL") || animationTrigger.m_Animator.GetBool("ShieldR")))//(2.2)
                    {
                        block();
                    }
                    else//(2.1)
                    {
                        attack();
                    }
                }
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
        if (randomHit < 20 || (!animationTrigger.m_Animator.GetBool("DeadRArm") && animationTrigger.m_Animator.GetBool("DeadLArm")))
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
        animationTrigger.m_Animator.SetFloat("HeadMovement", ((sideAngle - 60) / 60));
    }

    //Randomly attacks. Focuses on arms
    void attack()
    {
        if (debug)
            Debug.Log("Attack");
        float randomHit = Random.Range(0.0f, 40.0f);
        if (((randomHit < 20 && !animationTrigger.m_Animator.GetBool("DeadLArm")) || (animationTrigger.m_Animator.GetBool("DeadRArm") && !animationTrigger.m_Animator.GetBool("DeadLArm"))) && !animationTrigger.m_Animator.GetBool("ShieldL"))
        {
            if (randomHit < 10)
                animationTrigger.trigger("LSwing", true);
            else
                animationTrigger.trigger("LStab", true);

            animationTrigger.trigger("REmpty", true);
        }
        else if (!(animationTrigger.m_Animator.GetBool("DeadRArm")) && !animationTrigger.m_Animator.GetBool("ShieldR"))
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



    //Is called from Dismemberer. When specifically an arm falls off, that arm does not animate anymore
    public void DisablePart(string x)
    {
        GameObject weapon = null;
        if (x.Equals("LeftHand"))
        {
            if (animationTrigger.m_Animator.GetBool("ShieldR"))
            {
                if(holdsters[0]!=null)
                if (holdsters[0].transform.childCount > 0)
                    letGo(holdsters[0].transform.GetChild(0).gameObject);
                animationTrigger.setBool("DeadRArm", true);
                animationTrigger.setBool("ShieldR", false);
                deadArmsR = true;
            }
            if (holdsters[1] != null)
            {
                if (holdsters[1].transform.childCount > 0)
                    letGo(holdsters[1].transform.GetChild(0).gameObject);
                Destroy(holdsters[1]);
            }
            animationTrigger.setBool("DeadLArm", true);
            noArmL = true;
            canAttack = false;
        }
        else if (x.Equals("RightHand"))
        {
            if (animationTrigger.m_Animator.GetBool("ShieldL"))
            {
                if (holdsters[1] != null)
                    if (holdsters[1].transform.childCount > 0)
                    letGo(holdsters[1].transform.GetChild(0).gameObject);
                animationTrigger.setBool("DeadLArm", true);
                animationTrigger.setBool("ShieldL", false);
                deadArmsL = true;
            }
            animationTrigger.setBool("DeadRArm", true);
            noArmR = true;
            if (holdsters[0] != null)
            {
                if (holdsters[0].transform.childCount > 0)
                    letGo(holdsters[0].transform.GetChild(0).gameObject);
                Destroy(holdsters[0]);
            }
            canAttack = false;
        }
        else if (x.Contains("Foot"))
        {
            if (aDeadFoot)
            {
                this.GetComponent<CapsuleCollider>().height = 0.279099f;
                this.GetComponent<CapsuleCollider>().center = new Vector3(0, -0.1025473f, 0);
            }
            else
            {
                aDeadFoot = true;
            }
        }
        else if (x.Equals("LeftLeg"))
        {
            deadLegL = true;
        }
        else if (x.Equals("RightLeg"))
        {
            deadLegR = true;
        }
        if (deadLegR && deadLegL)
        {
            if (holdsters[0] != null)
            {
                if (holdsters[0].transform.childCount > 0)
                    letGo(holdsters[0].transform.GetChild(0).gameObject);
                Destroy(holdsters[0]);
            }
            if (holdsters[1] != null)
            {
                if (holdsters[1].transform.childCount > 0)
                    letGo(holdsters[1].transform.GetChild(0).gameObject);
                Destroy(holdsters[1]);
            }

                this.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
                this.gameObject.GetComponent<Collider>().enabled = false;
                animationTrigger.trigger("EndAnimation", false);
                GetComponent<HealthManagerEdit>().die();
                Destroy(this);
        }
        if (noArmR && noArmL)
        {
            if(!GetComponent<HealthManagerEdit>().dead)
                GetComponent<HealthManagerEdit>().die();
            Destroy(this);
        }
        }

    WeaponData findWeaponData(GameObject weapon)
    {
        if (weapon == null)
            return null;
        for(int i =0;i<weapon.transform.childCount;i++)
        {
            if (weapon.transform.GetChild(i) != null)
                if (weapon.transform.GetChild(i).gameObject.GetComponent<WeaponData>()!=null)
                    return weapon.transform.GetChild(i).gameObject.GetComponent<WeaponData>();
        }
        return weapon.GetComponent<WeaponData>();
    }
}
