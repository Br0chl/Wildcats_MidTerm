using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Workbench : MonoBehaviour
{
    [SerializeField] GameObject gunModel;


    [SerializeField] public List<GunStats> guns = new List<GunStats>();

    [SerializeField] GameObject activatePopUp;
    [SerializeField] GameObject statsUI;

    [SerializeField] TextMeshProUGUI damageUI;
    [SerializeField] TextMeshProUGUI maxAmmoUI;
    [SerializeField] TextMeshProUGUI startingMagsUI;

    [SerializeField] GameObject upgradeUI;

    [SerializeField] GameObject wbCamera;

    [SerializeField] TextMeshProUGUI gunLevelUI;
    [SerializeField] TextMeshProUGUI damageLevelUI;
    [SerializeField] TextMeshProUGUI damagePriceUI;
    [SerializeField] GameObject damageButton;
    [SerializeField] TextMeshProUGUI maxAmmoLevelUI;
    [SerializeField] TextMeshProUGUI maxAmmoPriceUI;
    [SerializeField] GameObject maxAmmoButton;
    [SerializeField] TextMeshProUGUI startingMagsLevelUI;
    [SerializeField] TextMeshProUGUI startingMagsPriceUI;
    [SerializeField] GameObject startingMagsButton;

    int gunToShow = 0;

    public bool isActivated = false;
    bool playerInTrigger = false;

    int damageBuyUpgradeLevel;
    int damageUpgradePrice;
    int maxAmmoBuyUpgradeLevel;
    int maxAmmoUpgradePrice;
    int startingMagsBuyUpgradeLevel;
    int startingMagsUpgradePrice;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInTrigger)
        {
            isActivated = !isActivated;
            ActivateWorkBench();
        }

        if (isActivated)
        {
            GunSelect();
        }
    }

    void ShowGun()
    {
        gunModel.GetComponent<MeshFilter>().sharedMesh = guns[gunToShow].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = guns[gunToShow].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        UpdateStatsPanel();

        UpdateUpgradePanel();
    }

    private void UpdateStatsPanel()
    {
        gunLevelUI.text = guns[gunToShow].gunLevel.ToString();
        damageUI.text = guns[gunToShow].shootDamage.ToString();
        int maxAmmo = guns[gunToShow].magCapacity * guns[gunToShow].maxMagazines;
        maxAmmoUI.text = maxAmmo.ToString();
        startingMagsUI.text = guns[gunToShow].startingMagazines.ToString();
    }

    void GunSelect()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunToShow < guns.Count - 1)
        {
            gunToShow++;
            ShowGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunToShow > 0)
        {
            gunToShow--;
            ShowGun();
        }
    }

    void ActivateWorkBench()
    {
        if (isActivated)
        {
            gameManager.instance.isShopping = true;
            activatePopUp.SetActive(false);
            gameManager.instance.currencyWBench.text = gameManager.instance.playerScript.totalCurrency.ToString();
            gunModel.SetActive(true);
            gameManager.instance.playerScript.enabled = false;
            gameManager.instance.hudUI.SetActive(false);
            wbCamera.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            ShowGun();
            statsUI.SetActive(true);
            upgradeUI.SetActive(true);
        }
        else if (!isActivated)
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            gameManager.instance.playerScript.enabled = true;
            wbCamera.SetActive(false);
            gameManager.instance.hudUI.SetActive(true);
            gunModel.SetActive(false);
            statsUI.SetActive(false);
            upgradeUI.SetActive(false);
            activatePopUp.SetActive(true);
            gameManager.instance.playerScript.UpdateShootDamage();
            gameManager.instance.isShopping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        playerInTrigger = true;
        activatePopUp.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        playerInTrigger = false;
        isActivated = false;
        gunModel.SetActive(false);
        activatePopUp.SetActive(false);
    }

    void UpdateDamageUI()
    {
        damageBuyUpgradeLevel = guns[gunToShow].damageUpgradeLevel + 1;
        damageUpgradePrice = guns[gunToShow].costToUpgrade * damageBuyUpgradeLevel;

        if (guns[gunToShow].damageUpgradeLevel == 10)
        {
            damageLevelUI.faceColor = Color.red;
            damageLevelUI.text = "Max";
            damagePriceUI.text = "---";
            damageButton.SetActive(false);
            return;
        }
        else
        {
            damageLevelUI.faceColor = Color.white;
            damageButton.SetActive(true);
            damageLevelUI.text = damageBuyUpgradeLevel.ToString();
        }
        if (damageUpgradePrice > gameManager.instance.playerScript.totalCurrency)
        {
            damagePriceUI.faceColor = Color.red;
            damageButton.SetActive(false);
        }
        else
        {
            damagePriceUI.faceColor = Color.white;
            damageButton.SetActive(true);
        }

        damagePriceUI.text = damageUpgradePrice.ToString();
    }

    public void BuyDamage()
    {
        if (guns[gunToShow].damageUpgradeLevel != 10 && damagePriceUI.faceColor != Color.red)
        {
            gameManager.instance.playerScript.totalCurrency -= damageUpgradePrice;
            guns[gunToShow].shootDamage += 1;
            guns[gunToShow].damageUpgradeLevel += 1;
            UpdateUpgradePanel();
            UpdateStatsPanel();
            gameManager.instance.UpdateCurrencyUI();
        }
    }

    void UpdateMaxAmmoUI()
    {
        maxAmmoBuyUpgradeLevel = guns[gunToShow].maxAmmoUpgradeLevel + 1;
        maxAmmoUpgradePrice = guns[gunToShow].costToUpgrade * maxAmmoBuyUpgradeLevel;

        if (guns[gunToShow].maxAmmoUpgradeLevel == 10)
        {
            maxAmmoLevelUI.faceColor = Color.red;
            maxAmmoLevelUI.text = "Max";
            maxAmmoPriceUI.text = "---";
            maxAmmoButton.SetActive(false);
            return;
        }
        else
        {
            maxAmmoLevelUI.faceColor = Color.white;
            maxAmmoButton.SetActive(true);
            maxAmmoLevelUI.text = maxAmmoBuyUpgradeLevel.ToString();
        }
        if (maxAmmoUpgradePrice > gameManager.instance.playerScript.totalCurrency)
        {
            maxAmmoPriceUI.faceColor = Color.red;
            maxAmmoButton.SetActive(false);
        }
        else
        {
            maxAmmoPriceUI.faceColor = Color.white;
            damageButton.SetActive(true);
        }

        maxAmmoPriceUI.text = maxAmmoUpgradePrice.ToString();
    }

    public void BuyMaxAmmo()
    {
        if (guns[gunToShow].maxAmmoUpgradeLevel != 10 && maxAmmoPriceUI.faceColor != Color.red)
        {
            gameManager.instance.playerScript.totalCurrency -= maxAmmoUpgradePrice;
            guns[gunToShow].maxMagazines += 1;
            guns[gunToShow].maxAmmoUpgradeLevel += 1;
            UpdateUpgradePanel();
            UpdateStatsPanel();
            gameManager.instance.UpdateCurrencyUI();
        }
    }

    void UpdateStartingMagsUI()
    {
        startingMagsBuyUpgradeLevel = guns[gunToShow].startingMagsUpgradeLevel + 1;
        startingMagsUpgradePrice = guns[gunToShow].costToUpgrade * startingMagsBuyUpgradeLevel;

        if (guns[gunToShow].startingMagsUpgradeLevel == 10)
        {
            startingMagsLevelUI.faceColor = Color.red;
            startingMagsButton.SetActive(false);
            startingMagsLevelUI.text = "Max";
            startingMagsPriceUI.text = "---";
            return;
        }
        else
        {
            startingMagsLevelUI.faceColor = Color.white;
            startingMagsButton.SetActive(true);
            startingMagsLevelUI.text = startingMagsBuyUpgradeLevel.ToString();
        }
        if (startingMagsUpgradePrice > gameManager.instance.playerScript.totalCurrency)
        {
            startingMagsPriceUI.faceColor = Color.red;
            startingMagsButton.SetActive(false);
        }
        else
        {
            startingMagsPriceUI.faceColor = Color.white;
            startingMagsButton.SetActive(true);
        }

        startingMagsPriceUI.text = startingMagsUpgradePrice.ToString();
    }

    public void BuyStartingMags()
    {
        if (guns[gunToShow].startingMagsUpgradeLevel != 10 && startingMagsPriceUI.faceColor != Color.red)
        {
            gameManager.instance.playerScript.totalCurrency -= startingMagsUpgradePrice;
            guns[gunToShow].startingMagazines += 1;
            guns[gunToShow].startingMagsUpgradeLevel += 1;
            UpdateUpgradePanel();
            UpdateStatsPanel();
            gameManager.instance.UpdateCurrencyUI();
        }
    }

    void UpdateUpgradePanel()
    {
        UpdateDamageUI();
        UpdateMaxAmmoUI();
        UpdateStartingMagsUI();
    }
}
