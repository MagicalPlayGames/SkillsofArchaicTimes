using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDisplay : MonoBehaviour
{
    public GameObject[] objects;
    public Quaternion originalRotation;
    public Vector3 originalPosition;
    public Vector3[] originalPos;
    public Quaternion[] originalRots;
    public GameObject activeObject;
    public float moveSpeed;
    public float rotSpeed;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach(GameObject model in objects)
        {
            if (model != null)
            {
                originalPos[i] = model.transform.position;
                originalRots[i] = model.transform.rotation;
            }
            i++;
        }
        originalRotation = this.transform.rotation;
        originalPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.position += moveSpeed * Time.deltaTime * Vector3.forward;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                this.transform.position += moveSpeed * Time.deltaTime * Vector3.back;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.position += moveSpeed * Time.deltaTime * Vector3.up;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                this.transform.position += moveSpeed * Time.deltaTime * Vector3.down;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position += moveSpeed * Time.deltaTime * Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += moveSpeed * Time.deltaTime * Vector3.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Rotate(new Vector3(-1, 0, 0), rotSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Rotate(new Vector3(1, 0, 0), rotSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Rotate(new Vector3(0, 1, 0), rotSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Rotate(new Vector3(0, -1, 0), rotSpeed * Time.deltaTime);
        }
    }

    public void selectGameObject(GameObject selected)
    {
        int i = 0;
        this.transform.rotation = originalRotation;
        this.transform.position = originalPosition;
        this.transform.DetachChildren();
        foreach (GameObject model in objects)
        {
            if (selected != model)
            {
                model.transform.position = originalPos[i];
                model.transform.rotation = originalRots[i];
                model.GetComponent<Collider>().enabled = true;
            }
            else
            {
                model.transform.parent = this.transform;
                if (model.name.Contains("Far"))
                    model.transform.position = this.transform.position + new Vector3(0, -1.0f, 0);
                else if (model.tag == "Enemy")
                    model.transform.position = this.transform.position + new Vector3(0, 1.0f, 0);
                else
                    model.transform.position = this.transform.position;
                model.GetComponent<Collider>().enabled = false;
            }
            i++;
        }
        activeObject = selected;
    }
}
