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
    [SerializeField] GameObject ammoCover;

    private void Update()
    {
        PriceCheck(item);
        GunCheck(item);
        HealthCheck(item);
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

    public void GunCheck(StoreItem item)
    {
        if (item == null) return;
        if (item.isGun)
        {
            if (gameManager.instance.playerScript.gunList.Contains(item.gunStats))
            {
                ammoCover.SetActive(true);
                if (item.gunStats.currentMagazines == item.gunStats.maxMagazines)
                {
                    buyButton.SetActive(false);
                    price.faceColor = Color.red;
                    price.text = "MAX";
                }
                else
                {
                    buyButton.SetActive(true);
                    price.faceColor = Color.white;
                    price.text = item.price.ToString();
                }
            }
            else
            {
                ammoCover.SetActive(false);
            }
        }
    }

    public void HealthCheck(StoreItem item)
    {
        if (item == null) return;
        if (item.isHP)
        {
            if (gameManager.instance.playerScript.currentHP == gameManager.instance.playerScript.maxHP)
            {
                buyButton.SetActive(false);
                price.faceColor = Color.red;
                price.text = "FULL";
            }
            else
            {
                    buyButton.SetActive(true);
                    price.color = Color.white;
                    price.text = item.price.ToString();
            }
        }
    }

    public void PriceCheck(StoreItem item)
    {
        if (item == null) return;
        if (item.price > gameManager.instance.playerScript.totalCurrency)
        {
            buyButton.SetActive(false);
            price.faceColor = Color.red;
        }
        else
        {
            buyButton.SetActive(true);
            price.faceColor = Color.white;
        }
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
            if (gameManager.instance.playerScript.gunList.Count < 2)
            {
                gameManager.instance.playerScript.gunList.Add(item.gunStats);
                gameManager.instance.playerScript.InitialzeGun(gameManager.instance.playerScript.gunList[1]);
                gameManager.instance.playerScript.totalCurrency -= item.price;
                gameManager.instance.UpdateUI();
                return;
            }
            else if (gameManager.instance.playerScript.gunList.Count == 2 && !gameManager.instance.playerScript.gunList.Contains(item.gunStats))
            {
                //gameManager.instance.playerScript.gunList.Add(item.gunStats);
                gameManager.instance.playerScript.gunPickup(item.gunStats);
                gameManager.instance.playerScript.totalCurrency -= item.price;
                gameManager.instance.UpdateUI();
                return;
                // gameManager.instance.playerScript.InitialzeGun(gameManager.instance.playerScript.gunList[1]);
                //break;
            }

            foreach (GunStats gs in gameManager.instance.playerScript.gunList)
            {
                if (gs == item.gunStats)
                {
                    if (gs.isAmmoFull) return;

                    gameManager.instance.playerScript.totalCurrency -= item.price;

                    gameManager.instance.playerScript.gunPickup(item.gunStats);
                }
                // else if (gameManager.instance.playerScript.gunList.Count == 2 && gs != item.gunStats)
                // {
                //     //gameManager.instance.playerScript.gunList.Add(item.gunStats);
                //     gameManager.instance.playerScript.gunPickup(item.gunStats);
                //     gameManager.instance.playerScript.totalCurrency -= item.price;
                //    // gameManager.instance.playerScript.InitialzeGun(gameManager.instance.playerScript.gunList[1]);
                //     //break;
                // }
            }
            gameManager.instance.UpdateUI();
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