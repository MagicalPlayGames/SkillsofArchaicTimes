using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
	public Component[] partHealths;
	public int Timer = 600;
    // Start is called before the first frame update
    void Start()
    {
        partHealths = GetComponentsInChildren<HealthBodyPart>();
		Debug.Log("Length of array: " + partHealths.Length);
		foreach (HealthBodyPart partHP in partHealths)
		{
			Debug.Log(partHP.gameObject.name + " health: " + partHP.health);
		}
    }

    // Update is called once per frame
    void Update()
    {
        Timer--;
		if (Timer <= 0)
		{
			Timer = 600;
			//Should remove this once it's all good
			//Debug.Log(partHealths[1].gameObject.name + " health: " + partHealths[1].gameObject.GetComponent<HealthBodyPart>().health);
		}
    }
}
