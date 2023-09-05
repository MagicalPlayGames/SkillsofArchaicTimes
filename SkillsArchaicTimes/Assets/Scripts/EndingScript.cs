using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingScript : MonoBehaviour
{
    public int counter;
    public GameObject deadEnemy;
    public Text countText;
    // Start is called before the first frame update
    void Start()
    {
        countText.text = counter.ToString();
        if (counter > 0)
            StartCoroutine(spawnTheDead());
    }

    IEnumerator spawnTheDead()
    {
        yield return new WaitForSeconds(1);
        Instantiate(deadEnemy, this.transform.position, this.transform.rotation);
        counter--;
        if(counter>0)
            StartCoroutine(spawnTheDead());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
