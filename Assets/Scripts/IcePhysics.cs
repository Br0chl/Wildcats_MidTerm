using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePhysics : MonoBehaviour
{
    bool isIn;
    [SerializeField] int speed; // = player speed
    [SerializeField] int sliding; 

    void Update()
    {
        if (isIn == true)
        {
            //Slow down speed or extend momentum
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isIn = false;
        }
    }
}
