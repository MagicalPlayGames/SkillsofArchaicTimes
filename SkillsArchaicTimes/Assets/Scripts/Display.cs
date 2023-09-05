using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    public MainDisplay mainScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        if(mainScript.isActiveAndEnabled)
        mainScript.selectGameObject(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
