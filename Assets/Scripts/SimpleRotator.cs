using UnityEngine;
using System.Collections;

public class SimpleRotator : MonoBehaviour
{

    public Vector3 speed;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = speed;
    }
}