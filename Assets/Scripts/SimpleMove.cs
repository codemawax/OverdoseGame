using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private GameController gc;

	void Start ()
    {
        rb = GetComponent<Rigidbody> ();
        rb.velocity = speed * transform.up;
    }

}
