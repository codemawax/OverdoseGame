using UnityEngine;
using System.Collections;

public class CurveMover : MonoBehaviour {

    public float speed;
    public float angle;
    private Rigidbody rb;
    private GameController gc;
    Vector3 curveRotation;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.velocity = speed * transform.up;
        curveRotation = new Vector3(0, 0, 0.1F);

    }

    void Update()
    {
        transform.position = transform.position + new Vector3(Mathf.Sin(angle * Time.deltaTime)/100,  Mathf.Cos(angle * Time.deltaTime)/100, 0);
    }

}
