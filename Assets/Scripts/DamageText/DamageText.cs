using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText = null;

    public void DestroyText()
    {
        Destroy(gameObject);
    }

    public void SetValue(int amount)
    {
        damageText.text = string.Format("{0:0}", amount);
    }
}
