using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANNInterface : MonoBehaviour
{
    [SerializeField]
    public ANNParams parameters;
    public ANN1 network;

    private static bool userInput = false;
    private void Start()
    {
        int choice = 0;
        if (userInput)
        {
            Debug.Log("This ANN judges if a number is divisble by a number");
            while (choice < 3)
            {
                Debug.Log("Input Arguments as follows");
                Debug.Log("layerSize,nodeSizes per layer as an array,divisbleBy,numOfIterations,learningRate");
                Debug.Log("Example:\n3,[8 4 1],3,1000,0.3");
                string[] inputs = Console.ReadLine().Split(',');
                string[] nodeSize = inputs[1].Substring(1, inputs[1].Length - 2).Split(' ');
                int[] nodeSizeInputs = new int[nodeSize.Length];
                for (int i = 0; i < nodeSize.Length; i++)
                {
                    Int32.TryParse(nodeSize[i], out nodeSizeInputs[i]);
                }
                float[] inputsAsNums = new float[5];
                for (int i = 0; i < 5; i++)
                {
                    if (i != 1)
                    {
                        float.TryParse(inputs[i], out inputsAsNums[i]);
                    }
                }

                network.setNetwork((byte)inputsAsNums[0], nodeSizeInputs, (byte)inputsAsNums[2], (byte)inputsAsNums[3], inputsAsNums[4], 0);
                choice = 1;
                while (choice == 1)
                {
                    choice = 0;
                    network.resetWeights();
                    while (choice == 0)
                    {
                        Debug.Log("\nAccuracy: " + network.testOp() + "\n\n");
                        Debug.Log("Select an option:\n0: Run with current weights\n1: Reset weights and run\n2: Restart\n3: Exit ");
                        Int32.TryParse(Console.ReadLine(), out choice);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Algorithm comparison test initiated");
            int[] node0 = { 8, 4, 4, 2, 2, 1 };
            int[] node1 = { 6, 4, 4, 2, 2, 1 };
            int[] node2 = { 8, 2, 4, 3, 2, 1 };
            int[] node3 = { 4, 4, 3, 2, 2, 1 };
            int[] node4 = { 4, 4, 4, 2, 1, 1 };
            int[] node5 = { 2, 4, 4, 2, 1, 1 };
            int[] node6 = { 6, 6, 4, 2, 2, 1 };
            int[][] nodes = { node0, node1, node2, node3, node4, node5, node6 };
            float[] thisSet = new float[100];
            for (byte aN = 0; aN < 3; aN++)
            {
                string topInfo = "";
                float toptop = 0;
                for (byte l = 1; l < 6; l++)
                {
                    for (int n = 0; n < nodes.Length; n++)
                    {
                        for (float lR = 0.001f; lR < 1; lR *= 5)
                        {
                            network.setNetwork(l, nodes[n], 2, 10, lR, aN);
                            network.resetWeights();
                            float topOutput = 0;
                            for (int i = 0; i < 50; i++)
                            {
                                thisSet[i] = network.testOp();
                                if (topOutput < thisSet[i])
                                    topOutput = thisSet[i];
                            }
                            if (toptop < topOutput)
                            {
                                topInfo = "Algorithm: " + aN + "\nLayers:" + l + " Nodes Index: " + n + " Learning Rate: " + lR + "\nBest Output: " + topOutput;
                                toptop = topOutput;
                            }

                            //Debug.Log("Layers:" + l + " Nodes Index: " + n + " Learning Rate: " + lR + "\nBest Output: " + topOutput);
                        }
                    }
                }
                Debug.Log("Best learner:\n" + topInfo);
            }
        }
    }
}
