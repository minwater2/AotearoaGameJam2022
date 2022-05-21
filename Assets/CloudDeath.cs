using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudDeath : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cloud"))
        {
            Destroy(collision.gameObject);    
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cloud"))
        {
            Destroy(other.gameObject);    
        }
    }
}
