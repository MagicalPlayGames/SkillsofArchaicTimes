using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI1Algorithm : MonoBehaviour
{
    //Runs per operate action
    public int iterations = 0;
    //calc cost (N/A)
    public float[] costs;
    public int totalCosts = 0;
    //Accuracy
    private int success;
    private int total;

    //Neuron
    [Serializable]
    public struct node
    {
        //Calc for output
        public float curSum;
        //Store for later
        public float previousSum;
        //Not used
        public float outputBeforeActivation;
        //Output
        public float output;
        //current Weights from previous layer to this node
        public float[] currentWeights;
        //adjustment storage for weight backward propergation
        public float[] errorAdjustments;
    };

    [Serializable]
    public struct layer
    {
        public node[] nodes;
    }

    [Serializable]
    public struct brain
    {
        //Input
        public layer input;
        //Hidden Layers
        public layer[] layers;
        //Output (change to multiple outputs)
        public float output;
        //N/A
        public float outputBeforeActivation;
        //Expected output
        public float realOutput;
        public float learningRate;
    }

    public brain control;
    public float accuracy;
    //Press to operate
    public bool operate;

    public int divisbleBy;

    //This ANN calculates the probability a random number is divisble by divisbleBy

    void testOp()
    {
        costs = new float[iterations];
        for (int j = 0; j < iterations; j++)
        {
            int choice = UnityEngine.Random.Range(0, 256);
            string inputs = System.Convert.ToString(choice, 2);
            while (inputs.Length < 8)
                inputs = "0" + inputs;
            int output;
            if (choice % divisbleBy == 0)
                output = 1;
            else
                output = 0;
            bool outcome = processData(inputs, output);
            if (outcome)
                success++;
            else
                updateAllWeights();
            total++;
        }
        accuracy = ((float)success) / ((float)total);
        success = 0;
        total = 0;
    }
    void Start()
    {
    resetWeights();
        testOp();
    }

    //Implement me
    public bool processData(string inputs, int expectedOutcome)
    {
        readInput(inputs);
        writeOutput();
        readTrueOutput(expectedOutcome);
        return (Mathf.Abs(control.output - control.realOutput) < 0.5);
    }

    //sets every weight to a random value
    void resetWeights()
    {
        for(int i =0;i<control.layers.Length;i++)
        {
            for(int j = 0;j<control.layers[i].nodes.Length;j++)
            {
                for(int w = 0;w<control.layers[i].nodes[j].currentWeights.Length;w++)
                {
                    control.layers[i].nodes[j].currentWeights[w] = UnityEngine.Random.Range(0.0f, 1.0f);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(operate)
        {
            operate = false;
            testOp();
        }
    }

    //reads 10101 string input and puts it into the input layer, setting all input weights to 1
    public void readInput(string input)
    {
        char[] nums = input.ToCharArray();
        control.input.nodes = new node[nums.Length];
        int i = 0;
        foreach(char num in nums)
        {
            control.input.nodes[i] = new node();
            control.input.nodes[i].output = ((int)num) - 48;
            control.input.nodes[i].currentWeights = new float[control.layers[0].nodes.Length];
            for (int j = 0; j < control.layers[0].nodes.Length; j++)
                control.input.nodes[i].currentWeights[j] = 1.0f;
                i++;
        }
    }
    public void readTrueOutput(int output)
    {
            control.realOutput = output;
    }

    //input->h1.sum->sigmoid->h1 output->h2->h3->output->tanh(output)
    public void writeOutput()
    {
        control.output = 0;
        bool inputFlag = true;
        for (int i = 0; i < control.layers.Length; i++)
        {
            layer curLayer = control.layers[i];
            layer previousLayer;
            if (inputFlag)
            {
                previousLayer = control.input;
                inputFlag = false;
            }
            else
            {
                previousLayer = control.layers[i - 1];
            }
            for (int j = 0; j < curLayer.nodes.Length; j++)
            {
                node curNode = curLayer.nodes[j];
                    for (int w = 0; w < previousLayer.nodes.Length; w++)
                    {
                        node previousNode = previousLayer.nodes[w];
                        curNode.curSum += previousNode.output * previousNode.currentWeights[0];
                    }
                control.layers[i].nodes[j].outputBeforeActivation = control.layers[i].nodes[j].previousSum = curNode.curSum;
                control.layers[i].nodes[j].output = sigmoid(curNode.curSum);
                control.layers[i].nodes[j].curSum = 0;
            }
        }
        for(int i = 0;i<control.layers[control.layers.Length-1].nodes.Length;i++)
        {
            control.output += control.layers[control.layers.Length - 1].nodes[i].output * control.layers[control.layers.Length - 1].nodes[i].currentWeights[0];
        }
        control.output = tanh(control.output);
    }

    public void updateAllWeights()
    {
        for (int i = control.layers.Length-1; i > -1; i--)
        {
            layer curLayer = control.layers[i];
            for (int j = 0; j < curLayer.nodes.Length; j++)
            {
                node curNode = curLayer.nodes[j];
                float[] errorAdjust;
                if (i == control.layers.Length - 1)
                    //output layer to last hidden layer
                    control.layers[i].nodes[j].errorAdjustments = errorAdjust = findSingleError(curNode.currentWeights, control.realOutput - control.output);
                else
                {
                    //layer to layer
                    control.layers[i].nodes[j].errorAdjustments = errorAdjust = findMatrixErrors(curLayer.nodes, control.layers[i + 1].nodes);
                }

                for (int w = 0; w < curNode.currentWeights.Length; w++)
                {
                    //edit weights
                        control.layers[i].nodes[j].currentWeights[w] -= errorAdjust[w] * control.learningRate;
                }
            }
        }
    }

    private bool flag = false;
    //average loss function
    private float cost()
    {
        if (totalCosts == 100)
        {
            totalCosts = 0;
            flag = true;
        }
        costs[totalCosts] = loss(control.output,control.realOutput);
        float totalCost = 0;
        for(int i =0;i<100;i++)
        {
            if (!flag && i > totalCosts)
                break;
            totalCost += costs[i];
        }
            return totalCost / (totalCosts+1);
    }

    private float loss(float predicted, float expected)
    {
        return (1e-15f+expected) * Mathf.Log(1e-15f+predicted);
    }

    private float sigmoid(float x)
    {
        float EX = Mathf.Exp(x);
        return EX / (1 + EX);
    }
    private float tanh(float x)
    {
        float EX = Mathf.Exp(x);
        float negativeEX = Mathf.Exp(-x);
        return (EX - negativeEX) / (EX + negativeEX);
    }

    private float errorChangeOutput(float realOutput, float output)
    {
        return -(realOutput-output);
    }

    private float outputChangeNet(float output)
    {
        return 1 / (1 + Mathf.Exp(-output));
    }

    private float netChangeWeights(float target, float output, float hiddenOutput)
    {
            return -(target - output) * output * (1 - output) * hiddenOutput;
    }


    private float error(float predicted, float target)
    {
        return Mathf.Pow(predicted - target, 2);
    }

    private float errorPredictedDerivative(float predicted, float target)
    {
        return 2.0f * (predicted - target);
    }

    private float tanhDerivative(float x)
    {
        return tanh(x) * (1.0f - tanh(x));
    }

    private float sigmoidDerivative(float x)
    {
        return sigmoid(x) * (1.0f - sigmoid(x));
    }

    private float updateWeight(float weight, float grad, float learningRate)
    {
        return weight - learningRate * grad;
    }

    //Error1 = w1/totalWeights * error
    private float[] findSingleError(float[] weights,float error)
    {
        float totalWeights = 0;
        for (int i = 0; i < weights.Length; i++)
            totalWeights += weights[i];
        for (int i = 0; i < weights.Length; i++)
            weights[i] = (weights[i] / totalWeights) * error;

        return weights;
    }

    //weights = [weights1[],weights2[],...]
    //errors = [errorOfNextNode][curNode]
    //transpose errors

    //weights DOT errors
    //if errors > weights
        //outcome = weights of node *errorOfNextNode
    //else
        //outcome = 1weight per node (weights connected to curLayer curNode) * errorsOfWeightNum(not errorsOf1Weight)
    private float[] findMatrixErrors(node[] curLayer, node[] upperLayer)
    {
        //curLayer for weights
        //upperLayer for errors

        //weights: [node][weight]
        //errors: [weights of node][node]


        float[][] weights = new float[curLayer.Length][];
        for (int i = 0; i < weights.Length; i++)
            weights[i] = curLayer[i].currentWeights;

        float[][] errors = new float[upperLayer[0].currentWeights.Length][];
        for(int i =0;i<errors.Length;i++)
        {
            errors[i] = new float[upperLayer.Length];
            for (int j = 0; j < errors[i].Length; j++)
            {
                errors[i][j] = upperLayer[j].errorAdjustments[i];
            }
        }
            errors = transpose(errors);



        //Dot Product
        int size;
        if (errors.Length > weights.Length)
            size = errors.Length;
        else
            size = weights.Length;

        float[] outcomes = new float[size];

        for (int i =0;i<size;i++)
        {
            outcomes[i] = 0;
            if (size == errors.Length)
            {
                for (int j = 0; j < errors[i].Length; j++)
                {
                    if (i >= weights.Length)
                        break;
                    for (int w = 0; w < weights[i].Length; w++)
                    {
                        outcomes[i] += weights[i][w] * errors[i][j];
                    }
                }
            }
            else
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    if (i >= errors.Length)
                        break;
                    for (int w = 0; w < errors[i].Length; w++)
                    {
                        outcomes[i] += weights[j][i] * errors[i][w];
                    }
                }
            }
        }
        return outcomes;
    }

    //flip matrix on its side
    private float[][] transpose(float[][] weights)
    {
        float[][] newWeights = new float[weights[0].Length][];
        for(int i =0;i<weights[0].Length;i++)
        {
            newWeights[i] = new float[weights.Length];
            for (int j =0;j<weights.Length;j++)
            {
                newWeights[i][j] = weights[j][i];
            }
        }
        return newWeights;
    }

    //0 //0
    //0 //0  //0 
    //0 //0
    //Error of h1 w1 = w1/(w1+w2)*Error of output
    //Eorror of h1 w1 = w2/(w1+w2)*Error of output


    //For multiple outputs
    //w1,2 weight 1 of output 2
    //Error of h1 w1,2 = Error of h1 w1 + (w1,2/(w1,2+w2,2)*Error of output 2


    //This is equal to
    //[w1,1 w2,1] dot [e1] = [eh1]
    //[w1,2 w2,2]     [e2]   [eh2]
    //weight matrix transposed

    //To transpose
    //for(i<rows)
        //for (j<collums)
        //data[j][i] = data[i][j]
}

