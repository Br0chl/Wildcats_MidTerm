using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public SphereCollider col;
    public Rigidbody rb;
    public bool damage;
    public bool speed;
    public bool invincible;
    public bool doubleCurrency;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(Powerup(other.gameObject));
        }
    }

    IEnumerator Powerup(GameObject powerup)
    {
        if (damage)
        {
            gameManager.instance.isDamageUp = true;
            col.enabled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            yield return new WaitForSeconds(10f);
            gameManager.instance.isDamageUp = false;
            Destroy(gameObject);
        }

        if (speed)
        {
            gameManager.instance.isSpeedUp = true;
            col.enabled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            yield return new WaitForSeconds(10f);
            gameManager.instance.isSpeedUp = false;
            Destroy(gameObject);
        }

        if (doubleCurrency)
        {
            gameManager.instance.isDoubleCurrency = true;
            col.enabled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            yield return new WaitForSeconds(10f);
            gameManager.instance.isDoubleCurrency = false;
            Destroy(gameObject);
        }

        if (invincible)
        {
            gameManager.instance.isInvinvcible = true;
            col.enabled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            yield return new WaitForSeconds(10f);
            gameManager.instance.isInvinvcible = false;
            Destroy(gameObject);
        }
    }
}
