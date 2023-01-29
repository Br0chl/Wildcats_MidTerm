using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public StoreItem item;

    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI iName;
    [SerializeField] TextMeshProUGUI price;
    [SerializeField] GameObject buyButton;

    private void Update()
    {
        PriceCheck(item);
    }

    public void AddItem(StoreItem newItem)
    {
        item = newItem;

        icon.sprite = newItem.icon;
        icon.enabled = true;
        iName.text = newItem.iName;
        PriceCheck(item);
        price.text = newItem.price.ToString();
    }

    public void PriceCheck(StoreItem item)
    {
        if (item == null) return;
        if (item.price > gameManager.instance.playerScript.totalCurrency)
            price.faceColor = Color.red;
        else
            price.faceColor = Color.white;
    }

    public void ClearSlot()
    {
        icon.enabled = false;
        iName.text = "";
        price.text = "";
        buyButton.SetActive(false);
    }

    public void Buy()
    {
        if (item.isGun)
        {
            if (item.price > gameManager.instance.playerScript.totalCurrency)
                return;
            foreach (GunStats gs in gameManager.instance.playerScript.gunList)
            {
                if (gs == item.gunStats)
                {
                    if (gs.isAmmoFull) return;

                    gameManager.instance.playerScript.totalCurrency -= item.price;

                    gameManager.instance.playerScript.gunPickup(item.gunStats);
                }
                else
                    gameManager.instance.playerScript.gunPickup(item.gunStats);
            }
        }

        if (item.isUpgrade)
        {
            if (item.price > gameManager.instance.playerScript.totalCurrency)
                return;

            gameManager.instance.playerScript.totalCurrency -= item.price;
            gameManager.instance.playerScript.maxHP += item.hpToAdd;

            gameManager.instance.playerScript.Heal(item.hpToAdd);

            gameManager.instance.UpdateUI();
        }

        if (item.isHP)
        {
            if (item.price > gameManager.instance.playerScript.totalCurrency)
                return;
            if (gameManager.instance.playerScript.currentHP == gameManager.instance.playerScript.maxHP)
                return;

            gameManager.instance.playerScript.totalCurrency -= item.price;

            gameManager.instance.playerScript.Heal(item.hpToAdd);
        }

        gameManager.instance.UpdateUI();

    }

}