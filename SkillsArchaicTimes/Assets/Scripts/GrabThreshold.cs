using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class GrabThreshold : MonoBehaviour
{
    //[SerializeField] private string grip;
    public Animator m_animator;
    private float[] thresholds = new float[2];
    private float[] prevThresholds = new float[2];
    public float[] deadRanges;
    public bool rightHand;

    private List<UnityEngine.XR.InputDevice> handDevices = new List<UnityEngine.XR.InputDevice>();
    private UnityEngine.XR.InputDevice device;
    // Start is called before the first frame update
    void Start()
    {
        if (rightHand)
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, handDevices);
        else
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, handDevices);
        device = handDevices[0];
    }

    // Update is called once per frame
    void Update()
    {
        float threshold = 0;
        float prevThreshold = prevThresholds[0];
        float deadRange = deadRanges[0];
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out threshold))
        {
            if (threshold - deadRange > prevThreshold || deadRange + threshold < prevThreshold)
            {
                prevThresholds[0] = threshold;
                m_animator.SetFloat("FingerThreshold", 1 - threshold);
                m_animator.SetTrigger("CloseF");
            }
            else if (deadRange + threshold < prevThreshold)
            {
                prevThresholds[0] = threshold;
                m_animator.SetFloat("FingerThreshold", 1- threshold);
                m_animator.SetTrigger("OpenF");
            }
        }
        prevThreshold = prevThresholds[1];
        deadRange = deadRanges[1];
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out threshold))
        {
            if (threshold - deadRange > prevThreshold)
            {
                prevThresholds[1] = threshold;
                m_animator.SetFloat("PointerThreshold", 1 - threshold);
                m_animator.SetTrigger("CloseP");
            }
            else if (deadRange + threshold < prevThreshold)
            {
                prevThresholds[1] = threshold;
                m_animator.SetFloat("PointerThreshold", 1- threshold);
                m_animator.SetTrigger("OpenP");
            }
        }

    }
}
