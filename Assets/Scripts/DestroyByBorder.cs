using UnityEngine;
using System.Collections;

public class DestroyByBorder : MonoBehaviour
{

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            Destroy(other.gameObject);
        }

    }
}
