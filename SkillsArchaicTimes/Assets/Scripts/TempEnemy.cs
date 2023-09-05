using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy : MonoBehaviour
{
    public Vector3 maxRange;
    public Vector3 minRange;
    public float movingDistance;
    public float maxDistance;
    public float moveSpeed;
    public bool right = false;
    private Vector3 currentPoint;
    // Start is called before the first frame update
    void Start()
    {
        currentPoint = this.transform.position;
        maxRange.y = this.transform.position.y;
        minRange.y = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (right)
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position -= transform.right * moveSpeed * Time.deltaTime;
        }
        if (Vector3.Distance(transform.position, currentPoint) > maxDistance)
        {
            transform.position = currentPoint;
        }
        else if (Vector3.Distance(transform.position,currentPoint)>=movingDistance)
        {
            right = !right;
        }  
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Weapon")
        {
            Vector3 RandomPos = new Vector3(Random.Range(minRange.x, maxRange.x), minRange.y, Random.Range(minRange.z, maxRange.z));
            transform.position = RandomPos;
            currentPoint = transform.position;
        }
    }
}
