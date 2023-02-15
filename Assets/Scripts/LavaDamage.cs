using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{

    [SerializeField] int lavaDmg;
    [SerializeField] int Health; // = player hp 
    bool isIn;

    private void Update()
    {
        if(isIn == true)
        {
            Health -= lavaDmg;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
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
