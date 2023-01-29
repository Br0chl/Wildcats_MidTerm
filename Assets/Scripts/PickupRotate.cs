using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * rotationSpeed, 0); 
    }
}
