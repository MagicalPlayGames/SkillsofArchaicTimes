using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyANN : MonoBehaviour
{
    public ANN2 mainNetwork;
    public ANN2 attackNetwork;
    public ANN2 blockNetwork;
    public ANN2 moveNetwork;
    public ANNParams[] parameters;

    public float[] inputs;
    public float distance, timeWaited = 0;
    public byte didHit, numOfWeapons, pickedUp, attacked, blocked, evaded = 0;
    public byte gotHit = 1;


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
    private Transform curItem;

    [SerializeField]
    public enum movement { None, Walk, Run, StepLeft, StepRight, LFStance, RFStance, LSStance, RSStance };
    [SerializeField]
    public enum combat { None, Stab, Swing, BlockMid, BlockHigh };

    public enum tasks { Completed, MoveToLocation, Attack, PickupWeapon, Block };

    [System.Serializable]
    public struct legDecisions
    {
        public movement legs;
        public Vector3 target;
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
    public tasks currentTask = tasks.Completed;
    public bool actionFinished = true;
    private int selectOutput = 0;
    public float[] outputs;

    // Start is called before the first frame update
    void Start()
    {
        openChace(Caching.cacheCount);
        mainNetwork.setNetwork(parameters[0]);
        mainNetwork.resetWeights();

        attackNetwork.setNetwork(parameters[1]);
        attackNetwork.resetWeights();

        blockNetwork.setNetwork(parameters[2]);
        blockNetwork.resetWeights();

        //moveNetwork.setNetwork(parameters[3]);
        //moveNetwork.resetWeights();

        inputs = setInputs();
        currentDecisions = selectAction();
        timeWaited = 2;
        StartCoroutine(makeDecisions());
    }

    private byte decideBestOutput(float[] outputs)
    {
        byte index = 0;
        float best = 1;
        for (byte i = 0; i < outputs.Length; i++)
        {
            if (Mathf.Abs(outputs[i]-1) < best)
            {
                index = i;
                best = Mathf.Abs(outputs[i] - 1);
            }
        }
        return index;
    }

    private Decisions selectAction()
    {
        mainNetwork.readInput(inputs);
        outputs = mainNetwork.writeOutputs();
        byte index = decideBestOutput(outputs);
        //Debug.Log(index);
        gotHit = 1;
        didHit = pickedUp = attacked = blocked = evaded = 0;
        Decisions decided = new Decisions();
        decided.legs.legs = movement.None;
        decided.arms.leftArm = decided.arms.rightArm = combat.None;
        if (index == 0 || (currentDecisions.arms.attachedWeapons[0]==null && currentDecisions.arms.attachedWeapons[1] == null))
        {
            if (numOfWeapons < 2)
            {
                curItem = findItem(GameObject.FindGameObjectsWithTag("Weapon")).transform;
                currentDecisions.legs.target = curItem.position;
                decided.arms = decideArms(true);
            }
            else
            {
                currentDecisions.legs.target = player.position;
                decided.arms = decideArms(false);
            }
            currentTask = tasks.MoveToLocation;
        }
        else if (index == 1)
        {
            currentDecisions.legs.target = player.position;
            currentTask = tasks.Block;
            decided.arms = decideArms(false);
        }
        else if(index==2)
        {
            currentTask = tasks.Attack;
            decided.arms = decideArms(true);
        }
        else
        {
            currentDecisions.legs.target = player.position;
            decided.arms = decideArms(true);
        }
        decided.legs = decideLegs();

        if (decided.arms.leftArm != combat.None || decided.arms.rightArm != combat.None)
            animationTrigger.armAttack();
        decided.arms.attachedWeapons = currentDecisions.arms.attachedWeapons;
        decided.arms.holdsters = currentDecisions.arms.holdsters;
        return decided;
    }

    //Distance from player
    //just got hit
    //just hit someone
    //num of weapons
    //last action was pick up 0
    //last action was attack 1
    //last action was block 2
    //last action was evade 3
    //time waited
    private float[] setInputs()
    {
        distance = Vector3.Distance(player.position, this.transform.position);
        float[] actions = { distance, gotHit, didHit, numOfWeapons, pickedUp, attacked, blocked, evaded, timeWaited };
        return actions;
    }

    private void Update()
    {
        lookAtTarget();
        if (shouldRotate(currentDecisions.legs.target))
            rotate(currentDecisions.legs.target, stepSpeed);
        if (actionDone())
            runTask(currentTask);
        if (actionFinished == false && currentTask == tasks.MoveToLocation)
        {
            currentDecisions.legs = decideLegs();
            move(currentDecisions.legs);
        }

        for (int i = 0; i < 2; i++)
            if (currentDecisions.arms.attachedWeapons[i] != null)
                if (currentDecisions.arms.attachedWeapons[i].GetComponent<WeaponData>().hitSomething == 1)
                    didHit = 1;
    }

    private void runTask(tasks currentTask)
    {
        actionFinished = false;
        if (currentTask != tasks.Completed)
        {
            if (currentTask == tasks.MoveToLocation)
            {
                currentDecisions.legs = decideLegs();
            }
            else if (currentTask == tasks.Attack || currentTask == tasks.PickupWeapon)
            {
                currentDecisions.arms = decideArms(true);
                //animationTrigger.armAttack();
            }
            else if (currentTask == tasks.Block)
                currentDecisions.arms = decideArms(false);
            //Debug.Log(currentTask);

        }
        this.currentTask = taskFinished(currentTask);
        if (this.currentTask == tasks.Completed)
            actionFinished = true;
    }

    private tasks taskFinished(tasks currentTask)
    {
        if (currentTask == tasks.MoveToLocation)
        {
            if (!shouldRotate(currentDecisions.legs.target) && Vector3.Distance(currentDecisions.legs.target, this.transform.position) <= minWalkDistance + 1)
            {
                //Debug.Log("Stop");
                return tasks.Completed;
            }
        }
        else if (currentTask == tasks.Attack)
            return tasks.Completed;
        else if (currentTask == tasks.PickupWeapon && pickedUp == 1)
        {
            return tasks.Completed;
        }
        else if (currentTask == tasks.Block)
            return tasks.Completed;
        return currentTask;
    }

    void assesDecision()
    {
        float distanceDecreased = 0;
        if (mainNetwork.getInput(0) > distance)
            distanceDecreased = 1;
        if ((attacked == 1))
        {
            if (didHit ==0)
            {
                mainNetwork.updateAllWeights();
                attackNetwork.updateAllWeights();
                Debug.Log("Bad Attack" + gameObject.name);
            }
        }
        else if (blocked == 1)
        {
            if (gotHit == 1 && didHit == 0)
            {
                mainNetwork.updateAllWeights();
                blockNetwork.updateAllWeights();
                Debug.Log("Bad Block" + gameObject.name);
            }
        }
        else if (blocked == 0 && attacked == 0 && distanceDecreased == 0)
        {
            mainNetwork.updateAllWeights();
            Debug.Log("Bad Action" + gameObject.name);
        }
        else
        {
            Debug.Log("GoodPlayer" + gameObject.name);
        }
    }

    IEnumerator makeDecisions()
    {
        animationTrigger.setInt("MoveType", 0);
        animationTrigger.setInt("StanceType", 0);
        animationTrigger.setInt("LArmAction", 0);
        animationTrigger.setInt("RArmAction", 0);
        if (actionDone())
        {

            float[] trueOutputs = { gotHit, didHit, pickedUp };
            mainNetwork.readTrueOutput(trueOutputs);
            assesDecision();
            inputs = setInputs();
            currentDecisions = selectAction();
            timeWaited = 2;
        }
        yield return new WaitForSeconds(2);
        //Attack
        //Block
        //Pickup
        //Run

        StartCoroutine(makeDecisions());
    }

    //Decides if the target is far enough to rotate towards them
    bool shouldRotate(Vector3 target)
    {
        Vector3 targetDirection = target - transform.position;
        targetDirection.y = 0;
        bool should = Vector3.Angle(transform.forward, targetDirection) > minRotateAngle;
        return should;
    }

    //Rotates around
    void rotate(Vector3 targ, float speed)
    {
        Vector3 targetDirection = targ - transform.position;
        targetDirection.y = 0;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, speed * speedMultiplier * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection, new Vector3(0, 20, 0));
    }

    private legDecisions decideLegs()
    {
        legDecisions decided;
        decided.target = currentDecisions.legs.target;
        if (Vector3.Distance(decided.target, this.transform.position) > minRunDistance)
        {
            decided.legs = movement.Run;
        }
        else if (Vector3.Distance(decided.target, this.transform.position) > minWalkDistance)
        {
            decided.legs = movement.Walk;
        }
        else if (shouldRotate(decided.target))
        {
            decided.legs = stepDirection(decided.target);
        }
        else
        {
            decided.legs = movement.None;
            actionFinished = true;
        }
        return decided;
    }

    private movement stepDirection(Vector3 target)
    {
        Vector3 targetDirection = target - transform.position;
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

    //move to location task
    private void move(legDecisions decision)
    {
        if (decision.legs == movement.StepLeft || decision.legs == movement.StepRight)
            sideStep(decision.legs);
        else if (decision.legs == movement.Walk)
            walkTo(decision.target);
        else if (decision.legs == movement.Run)
            runTo(decision.target);
    }

    private armDecisions decideArms(bool attack)
    {
        armDecisions decided;
        decided.leftArm = combat.None;
        decided.rightArm = combat.None;
        decided.attachedWeapons = currentDecisions.arms.attachedWeapons;
        decided.holdsters = currentDecisions.arms.holdsters;
        if (curItem != null)
        {
            if (decided.attachedWeapons[0] == null)
                StartCoroutine(pickUpItem(curItem, 0));
            else
                StartCoroutine(pickUpItem(curItem, 1));
        }
        else if (decided.attachedWeapons[0] != null)
            decided.leftArm = decideArm(attack);
        else
            decided.rightArm = decideArm(attack);

        animationTrigger.setInt("RArmAction", (int)decided.rightArm);
        animationTrigger.setInt("LArmAction", (int)decided.leftArm);
        return decided;
    }


    //find and pickup weapon task

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
        numOfWeapons++;
        pickedUp = 1;
        currentDecisions.legs.target = player.position;
        curItem = null;
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

    //attack task
    private combat decideArm(bool attack)
    {
        combat decided;
        int randomAttack;
        float[] inputs = { distance, gotHit, didHit };
        if (attack)
        {
            attackNetwork.readInput(inputs);
            outputs = attackNetwork.writeOutputs();
            byte index = decideBestOutput(outputs);
            randomAttack = (index%2)+1;
        }
        else
        {
            blockNetwork.readInput(inputs);
            outputs = blockNetwork.writeOutputs();
            byte index = decideBestOutput(outputs);
            randomAttack = ((index%2)+4);
        }
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

    private void walkTo(Vector3 target)
    {
        animationTrigger.setInt("MoveType", 1);
        transform.position += transform.forward * walkSpeed * speedMultiplier * Time.deltaTime;
    }

    private void runTo(Vector3 target)
    {
        animationTrigger.setInt("MoveType", 2);
        transform.position += transform.forward * runSpeed * speedMultiplier * Time.deltaTime;
    }

    private bool actionDone()
    {
        return animationTrigger.getBool("ArmsFinished") && actionFinished;
    }
    void lookAtTarget()
    {

        Vector3 targetDirection = currentDecisions.legs.target - transform.position;
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

    private Vector3 positionCloseToTarget(Vector3 position)
    {
        Vector3 location = new Vector3(
            Random.Range(position.x - minWalkDistance, position.x + minWalkDistance),
            Random.Range(position.y - minWalkDistance, position.y - minWalkDistance),
            position.z);
        return location;
    }

    void openChace(int index)
    {
        Directory.CreateDirectory("Cache"+(index+1));

        Caching.AddCache("Cache" + (index+1));
    }
    void ClearCache()
    {
        bool success = Caching.ClearCache();

        if (!success)
        {
            Debug.Log("Unable to clear cache");
        }
    }
    void OnApplicationQuit()
    {
        ClearCache();
    }
    ~EnemyANN()
        {
        ClearCache();
    }
}
