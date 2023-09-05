using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SingleANN : MonoBehaviour
{
    int attackCount = 0;
    int blockCount = 0;
    int didHitCount = 0;
    int gotHitCount = 0;
    int totalCount = 0;
    public ANN2 mainNetwork;
    public ANNParams parameters;

    public float[] inputs;
    public byte didHit, attacked, blocked, gotHit = 0;

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

    public bool actionFinished = true;
    public float[] outputs;

    private void Start()
    {
        openChace(Caching.cacheCount);
        mainNetwork.setNetwork(parameters);
        mainNetwork.resetWeights();
        //IE for Making decisions
    }

    //Decision:
        //Walk
        //Attack
        //Block

    void openChace(int index)
    {
        Directory.CreateDirectory("Cache" + (index + 1));

        Caching.AddCache("Cache" + (index + 1));
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
    ~SingleANN()
    {
        ClearCache();
    }
}

