using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 newPosition = gameManager.instance.player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, gameManager.instance.player.transform.eulerAngles.y, 0f);
    }
}
