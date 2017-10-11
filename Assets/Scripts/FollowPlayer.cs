using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{

    private Transform player;
    private GameObject playerObject;
    public float timeAlive = 10.0F;
    public float speed = 2.5F;
    private float startTime;

    public void Start ()
    {
        startTime = Time.time;
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update ()
    {

        if (Time.time - startTime > timeAlive)
        {
            Destroy(this.gameObject);
        }
        transform.position = Vector3.MoveTowards(transform.position, playerObject.transform.position, speed * Time.deltaTime);
	}
}
