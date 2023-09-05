using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ButtonScript : MonoBehaviour
{
    //This item that can be interacted with
    XRBaseInteractable m_Interactable;

    private float startingPercentage = 0.0f;
    private float currentPercentage = 0.0f;
    private Vector3 grabPos;

    public GameObject start;
    public GameObject end;
    public Rigidbody myBody;
    public float springSpeed;


    public bool pressed = false;
    public bool toChange = false;
    public GameObject toDisable;
    public int sceneChange;



    //Interactable.typeOfInteraction.AddListener(your listener)
    //Everytime the interactable is hovered, OnFirstHoverEntered is used
    protected void OnEnable()
    {
        m_Interactable = GetComponent<XRBaseInteractable>();
        
        m_Interactable.firstHoverEntered.AddListener(OnFirstHoverEntered);
    }


    private void Update()
    {
        if (m_Interactable.isHovered)
        {
            UpdateYPosition();
        }
        //return back to the start position, in an almost spring upwards motion.
        else if(myBody.transform.position!= start.transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, start.transform.position, springSpeed * Time.deltaTime);

            Vector3 targetDirection = end.transform.position - start.transform.position;
            float length = targetDirection.magnitude;
            targetDirection.Normalize();
            float newPercentage = startingPercentage + (Vector3.Dot(transform.position, targetDirection) / length);
            currentPercentage = Mathf.Clamp01(newPercentage);
        }
    }

    protected void OnDisable()
    {
        m_Interactable.firstHoverEntered.RemoveListener(OnFirstHoverEntered);
    }

    //Finds the start percentage and start grab position on the first time it hovered on
    protected virtual void OnFirstHoverEntered(HoverEnterEventArgs args)
    {
        startingPercentage = currentPercentage;
        grabPos = args.interactorObject.transform.position;
    }
    //Updates the position of the button
    protected void UpdateYPosition()
    {
        if (currentPercentage < .90)
        {
            pressed = false;
            float newPercentage = startingPercentage + FindPercentageDifference();
            currentPercentage = Mathf.Clamp01(newPercentage);
            myBody.MovePosition(Vector3.Lerp(start.transform.position, end.transform.position, newPercentage));
        }
        else if (!pressed)
        {
            Debug.Log("Yes");
            pressed = true;
            press();
        }
    }

    void press()
    {
        if (toChange)
            SceneManager.LoadScene(sceneChange);
        else if (toDisable!=null)
            toDisable.SetActive(false);
        else
            Application.Quit();
    }

    //Finds the percetange of this position related to start(0) - end(1)
    protected float FindPercentageDifference()
    {
        Vector3 handPos = m_Interactable.interactorsHovering[0].transform.position;
        Vector3 pullDirection = handPos - grabPos;
        Vector3 targetDirection = end.transform.position - start.transform.position;
        float length = targetDirection.magnitude;
        targetDirection.Normalize();
        return Vector3.Dot(pullDirection, targetDirection) / length;
    }

}
