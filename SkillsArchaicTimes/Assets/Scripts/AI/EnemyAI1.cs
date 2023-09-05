using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyAI1 : MonoBehaviour
{
    public BasicAnimationController animationTrigger;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float minRunDistance;
    [SerializeField]
    private float minWalkDistance;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float stepSpeed;
    [SerializeField]
    private float speedMultiplier;
    [SerializeField]
    private float minRotateAngle;
    [SerializeField]
    private Transform player;

    [SerializeField]
    public enum movement { None, Walk, Run, StepLeft, StepRight, LFStance, RFStance, LSStance, RSStance };
    [SerializeField]
    public enum combat { None, Stab, Swing, BlockMid, BlockHigh };

    [System.Serializable]
    public struct legDecisions
    {
        public movement legs;
        public Transform target;
    };
    [System.Serializable]
    public struct armDecisions
    {
        public GameObject[] attachedWeapons;
        public combat leftArm;
        public combat rightArm;
        public GameObject[] holdsters;
    };

    [System.Serializable]
    public struct Decisions
    {
        public legDecisions legs;
        public armDecisions arms;
    };

    public Decisions currentDecisions;

    private void Update()
    {
        lookAtTarget();
        if (shouldRotate(currentDecisions.legs.target))
            rotate(currentDecisions.legs.target, stepSpeed);
        move(currentDecisions.legs);
    }

    IEnumerator makeDecisions()
    {
        animationTrigger.setInt("MoveType", 0);
        animationTrigger.setInt("StanceType", 0);
        animationTrigger.setInt("LArmAction", 0);
        animationTrigger.setInt("RArmAction", 0);
        if (actionDone())
            currentDecisions = setDecisions();
        yield return new WaitForSeconds(2);
        StartCoroutine(makeDecisions());
    }

    private void Start()
    {
        StartCoroutine(makeDecisions());
    }


    private Decisions setDecisions()
    {
        Decisions decided;
        if (currentDecisions.arms.attachedWeapons[0] == null && currentDecisions.arms.attachedWeapons[1] == null)
            currentDecisions.legs.target = findItem(GameObject.FindGameObjectsWithTag("Weapon")).transform;
        else
            currentDecisions.legs.target = player;
        decided.legs.target = currentDecisions.legs.target;
        decided.legs = decideLegs();
        if (decided.legs.legs == movement.None)
        {
            decided.arms = decideArms();
        }
        else
        {
            decided.arms = new armDecisions();
        }
        if (decided.arms.leftArm != combat.None || decided.arms.rightArm != combat.None)
            animationTrigger.armAttack();
        decided.arms.attachedWeapons = currentDecisions.arms.attachedWeapons;
        decided.arms.holdsters = currentDecisions.arms.holdsters;
        return decided;
    }

    //Decides if the target is far enough to rotate towards them
    bool shouldRotate(Transform target)
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0;
        bool should = Vector3.Angle(transform.forward, targetDirection) > minRotateAngle;
        return should;
    }

    //Rotates around
    void rotate(Transform targ, float speed)
    {
        Vector3 targetDirection = targ.position - transform.position;
        targetDirection.y = 0;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, speed * speedMultiplier * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection, new Vector3(0, 20, 0));
    }

    private legDecisions decideLegs()
    {
        legDecisions decided;
        decided.target = currentDecisions.legs.target;
        if (Vector3.Distance(decided.target.position, this.transform.position) > minRunDistance)
            decided.legs = movement.Run;
        else if (Vector3.Distance(decided.target.position, this.transform.position) > minWalkDistance)
            decided.legs = movement.Walk;
        else if (shouldRotate(decided.target))
        {
            decided.legs = stepDirection(decided.target);
        }
        else
        {
            decided.legs = movement.None;
        }
        return decided;
    }

    private movement stepDirection(Transform target)
    {
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.y = 0;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, stepSpeed * speedMultiplier * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection, new Vector3(0, 20, 0));
        Vector3 rightDir = Vector3.RotateTowards(transform.right, targetDirection, stepSpeed * speedMultiplier * Time.deltaTime, 0.0f);
        Vector3 leftDir = Vector3.RotateTowards(-transform.right, targetDirection, stepSpeed * speedMultiplier * Time.deltaTime, 0.0f);
        if (Vector3.Angle(rightDir, targetDirection) > Vector3.Angle(leftDir, targetDirection))//If enemy is on the right
            return movement.StepRight;
        else
            return movement.StepLeft;
    }

    private void move(legDecisions decision)
    {
        if (decision.legs == movement.StepLeft || decision.legs == movement.StepRight)
            sideStep(decision.legs);
        else if (decision.legs == movement.Walk)
            walkTo(decision.target);
        else if (decision.legs == movement.Run)
            runTo(decision.target);
    }

    private armDecisions decideArms()
    {
        armDecisions decided;
        decided.leftArm = combat.None;
        decided.rightArm = combat.None;
        decided.attachedWeapons = currentDecisions.arms.attachedWeapons;
        decided.holdsters = currentDecisions.arms.holdsters;
        if (currentDecisions.legs.target.tag == "Weapon")
            StartCoroutine(pickUpItem(currentDecisions.legs.target,0));
        else if (decided.attachedWeapons[0] != null)
            decided.leftArm = decideArm();
        else
            decided.rightArm = decideArm();

        animationTrigger.setInt("RArmAction", (int)decided.rightArm);
        animationTrigger.setInt("LArmAction", (int)decided.leftArm);
        return decided;
    }

    private IEnumerator pickUpItem(Transform item, int itemSlot)
    {
        animationTrigger.setInt("LArmAction", 6);
        yield return new WaitForSeconds(0.75f);
        if (!item.GetComponent<WeaponData>().grabbed)
        {
            currentDecisions.arms.attachedWeapons[itemSlot] = item.gameObject;
            GameObject handle = null;
            if (item.childCount > 0)
                for (int i = 0; i < item.childCount; i++)
                {
                    if (item.name.Contains("andle"))
                    {
                        handle = item.GetChild(i).gameObject;
                        break;
                    }
                }
            if (handle != null)
            {
                handle.transform.parent = null;
                currentDecisions.arms.attachedWeapons[itemSlot].transform.parent = handle.transform;
                setParent(handle, currentDecisions.arms.holdsters[itemSlot]);
            }
            else if (item.parent != null)
            {
                setParent(item.parent.gameObject, currentDecisions.arms.holdsters[itemSlot]);
            }
            else
            {
                setParent(item.parent.gameObject, currentDecisions.arms.holdsters[itemSlot]); ;
            }

            yield return new WaitForSeconds(0.75f);
            if (item.childCount == 0 && item.parent != null)
            {
                item.parent.GetComponent<Rigidbody>().useGravity = false;
                item.parent.GetComponent<Collider>().isTrigger = true;
                item.parent.GetComponent<Rigidbody>().isKinematic = true;
                item.parent.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
            }
            else
            {
                item.GetComponent<Rigidbody>().useGravity = false;
                item.GetComponent<Collider>().isTrigger = true;
                item.GetComponent<Rigidbody>().isKinematic = true;
                item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
            }
        }
        item.GetComponent<WeaponData>().grabbed = true;
    }
private void setParent(GameObject child, GameObject parent)
{
    if (child == null)
        return;
    if (parent != null)
        child.transform.parent = parent.transform;

    child.transform.localPosition = Vector3.zero;
    child.transform.localRotation = Quaternion.identity;
}

private combat decideArm()
    {
        combat decided;
        int randomAttack = Random.Range((int)combat.Stab, (int)combat.BlockHigh);
        decided = (combat)randomAttack;
        return decided;
    }

    //This is for how long the enemy steps to the side when that decision is made in closeRange()
    private IEnumerator stepTime()
    {
        float randomVar = Random.Range(0.0f, 5.0f);
        yield return new WaitForSeconds(randomVar);
        animationTrigger.setInt("MoveType", 0);
        currentDecisions.legs.legs = movement.None;
    }

    private void sideStep(movement stepType)
    {
        if (animationTrigger.getInt("MoveType") < 3)
            StartCoroutine(stepTime());

        if (stepType == movement.StepLeft)
        {
            animationTrigger.setInt("MoveType", 3);
            transform.position -= transform.right * stepSpeed * speedMultiplier * Time.deltaTime;
        }
        else
        {
            animationTrigger.setInt("MoveType", 4);
            transform.position += transform.right * stepSpeed * speedMultiplier * Time.deltaTime;
        }
    }

    private void walkTo(Transform target)
    {
        animationTrigger.setInt("MoveType", 1);
        transform.position += transform.forward * walkSpeed * speedMultiplier * Time.deltaTime;
    }

    private void runTo(Transform target)
    {
        animationTrigger.setInt("MoveType", 2);
        transform.position += transform.forward * runSpeed * speedMultiplier * Time.deltaTime;
    }

    private bool actionDone()
    {
        return animationTrigger.getBool("ArmsFinished");
    }
    void lookAtTarget()
    {

        Vector3 targetDirection = currentDecisions.legs.target.position - transform.position;
        targetDirection.y = 0;
        float sideAngle = Vector3.Angle(transform.right, targetDirection);
        animationTrigger.m_Animator.SetFloat("HeadMovement", ((sideAngle - 60) / 60));
    }

    private GameObject findItem(GameObject[] options)
    {
        float distance = 100;
        int index = -1;
        for (int i = 0; i < options.Length; i++)
        {
           // if (!exclusions.Contains(options[i]))
                float curDistance = Vector3.Distance(transform.position, options[i].transform.position);
                if (distance > curDistance)
                {
                    index = i;
                    distance = curDistance;
                }
        }
        if (index == -1)
            return null;
        return options[index];
    }
}
//ANIMATION PARAMETERS
        //MoveType
            //No Legs
            //0 Stance
            //1 Walk
            //2 Run
            //3 SideStep Left
            //4 SideStep Right
        //Stance Type
            //0 Stance
            //1 LF
            //2 LS
            //3 RF
            //4 RS
        //LArm Action
            //-1 No Arm
            //0 Stance
            //1 Stab Left
            //2 Swing Left
            //3 MidBlock Left
            //4 HighBlock Left
            //5 Shield
            //6 Pickup Item
        //RArm Action
            //-1 No Arm
            //0 Stance
            //1 Stab Right
            //2 Swing Right
            //3 MidBlock Right
            //4 HighBlock Right
            //5 Shield
            //6 Pickup Item
    //Find Weapon

    //Dont worry about variables until you are applying them
    
