using UnityEngine;
using System.Collections;

public class SimpleRotator : MonoBehaviour
{

    public float speed;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = Vector3.forward * speed;
    }
}