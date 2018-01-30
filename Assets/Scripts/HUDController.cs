using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct CastleUpgrades {

    public int costPerFortify;
    public int costPerRepair;
    public int pointsPerFortify;
    public int pointsPerRepair;

    public CastleUpgrades(int cf, int cr, int pf, int pr) {
        costPerFortify = cf;
        costPerRepair = cr;
        pointsPerFortify = pf;
        pointsPerRepair = pr;
    }
}

public class HUDController : MonoBehaviour {

    public Button fortifyArmor;
    public Button fortifyHealth;
    public Button repairArmor;
    public Button repairHealth;
    public CastleUpgrades armor = new CastleUpgrades(25, 15, 5, 10);
    public CastleUpgrades health = new CastleUpgrades(15, 5, 25, 15);
    public Color disabledArmor;
    public Color disabledHealth;
    public Color selectedColor;
    public List<GameObject> castles = new List<GameObject>();
    public GameObject gamePanel;
    public GameObject upgradePanel;
    public Image armorBar;
    public Image healthBar;
    public int[] castleCosts = { 0, 150, 500 };
    public Text armorText;
    public Text coinText;
    public Text healthText;
    public Text roundText;

    private ColorBlock da;
    private ColorBlock dh;
    private GameObject currentCastle;
    private GameObject nextCastle;

    public void Awake() {
        castles[1].GetComponent<Button>().onClick.AddListener(UpgradeCastle);
        castles[2].GetComponent<Button>().onClick.AddListener(UpgradeCastle);
        fortifyArmor.onClick.AddListener(FortifyArmor);
        fortifyHealth.onClick.AddListener(FortifyHealth);
        repairArmor.onClick.AddListener(RepairArmor);
        repairHealth.onClick.AddListener(RepairHealth);

        if (Game.Instance.data.castleLevel < castles.Count) {
            currentCastle = castles[Game.Instance.data.castleLevel - 1];
            nextCastle = castles[Game.Instance.data.castleLevel];
        }
    }

    private void Update() {
        if (upgradePanel && upgradePanel.activeSelf) {
            AlterButton(armor.costPerFortify, fortifyArmor, disabledArmor, -1, 0);
            AlterButton(armor.costPerRepair, repairArmor, disabledArmor, Game.Instance.data.currentArmor, Game.Instance.data.maxArmor);
            AlterButton(health.costPerFortify, fortifyHealth, disabledHealth, -1, 0);
            AlterButton(health.costPerRepair, repairHealth, disabledHealth, Game.Instance.data.currentHealth, Game.Instance.data.maxHealth);

            if (Game.Instance.data.castleLevel < castles.Count && Game.Instance.data.coins >= castleCosts[Game.Instance.data.castleLevel]) {
                Button nextButton = nextCastle.GetComponentInChildren<Button>();
                nextButton.interactable = true;
                nextButton.GetComponentInChildren<Text>().text = castleCosts[Game.Instance.data.castleLevel] + " coin";
            }
        }

        UpdateCoins(Game.Instance.data.coins);
    }

    private void AlterButton(int cost, Button button, Color color, int current, int max) {
        if (Game.Instance.data.coins < cost || current >= max) {
            button.interactable = false;
            button.GetComponent<Image>().color = color;
        } else {
            button.interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }
    }

    public void HideUpgradePanel() {
        upgradePanel.SetActive(false);
    }

    public void ShowUpgradePanel() {
        upgradePanel.SetActive(true);
    }

    void UpgradeCastle() {
        Button currentButton = currentCastle.GetComponentInChildren<Button>();
        Text nextText = nextCastle.GetComponentInChildren<Button>().GetComponentInChildren<Text>();

        currentButton.interactable = false;
        currentButton.GetComponentInChildren<Text>().enabled = false;
        nextText.text = "Selected";
        nextText.color = selectedColor;
        nextText.GetComponent<Outline>().enabled = false;
        nextCastle.GetComponentInChildren<Button>().GetComponentInChildren<Image>().enabled = false;
        Game.Instance.data.coins -= castleCosts[Game.Instance.data.castleLevel];
        Game.Instance.data.castleLevel += 1;

        currentCastle = nextCastle;

        if (Game.Instance.data.castleLevel < castles.Count) {
            nextCastle = castles[Game.Instance.data.castleLevel];

            if (Game.Instance.data.coins >= castleCosts[Game.Instance.data.castleLevel]) {
                nextCastle.GetComponentInChildren<Button>().interactable = true;
                nextCastle.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = castleCosts[Game.Instance.data.castleLevel] + " coin";
            } else {
                nextCastle.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Not enough coin!";
            }
        }

        UpgradeCastleImage(Game.Instance.data.castleLevel);
        UpdateCoins(Game.Instance.data.coins);
    }

    void UpgradeCastleImage(int level) {
        switch (level) {
            case 2:
                GameObject smallCastle = GameObject.Find("SmallCastle");
                smallCastle.transform.parent.GetChild(level - 1).gameObject.SetActive(true);
                smallCastle.SetActive(false);
                break;
            case 3:
                GameObject mediumCastle = GameObject.Find("MediumCastle");
                mediumCastle.transform.parent.GetChild(level - 1).gameObject.SetActive(true);
                mediumCastle.SetActive(false);
                break;
            default:
                break;
        }
    }

    void FortifyArmor() {
        Game.Instance.data.maxArmor += armor.pointsPerFortify;
        Game.Instance.data.coins -= armor.costPerFortify;
        Game.Instance.data.armorLevel += 1;
        UpdateArmor(Game.Instance.data.currentArmor);
        UpdateCoins(Game.Instance.data.coins);
    }

    void FortifyHealth() {
        Game.Instance.data.maxHealth += health.pointsPerFortify;
        Game.Instance.data.coins -= health.costPerFortify;
        Game.Instance.data.healthLevel += 1;
        UpdateHealth(Game.Instance.data.currentHealth);
        UpdateCoins(Game.Instance.data.coins);
    }

    void RepairArmor() {
        Game.Instance.data.currentArmor += armor.pointsPerRepair;
        Game.Instance.data.coins -= armor.costPerRepair;
        UpdateArmor(Game.Instance.data.currentArmor);
        UpdateCoins(Game.Instance.data.coins);
    }

    void RepairHealth() {
        Game.Instance.data.currentHealth += health.pointsPerRepair;
        Game.Instance.data.coins -= health.costPerRepair;
        UpdateHealth(Game.Instance.data.currentHealth);
        UpdateCoins(Game.Instance.data.coins);
    }

    public void UpdateArmor(float armor) {
        armorBar.fillAmount = armor / Game.Instance.data.maxArmor;
        if (armor >= 0) {
            armorText.text = ((int) armor).ToString();
        }
    }

    public void UpdateCoins(int coins) {
        coinText.text = coins.ToString();
    }
	
	public void UpdateHealth(float health) {
        healthBar.fillAmount = health / Game.Instance.data.maxHealth;
        if (health >= 0) {
            healthText.text = ((int) health).ToString();
        }
    }

    public void UpdateRoundText() {
        roundText.text = Game.Instance.data.round.ToString();
    }
}
