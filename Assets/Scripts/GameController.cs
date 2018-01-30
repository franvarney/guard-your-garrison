using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public Button exitButton;
    public Button startButton;
    public Canvas canvas;
    public GameObject boundary;
    public GameObject gameOverPanel;
    public float roundTime = 30.0f;
    public Spawner spawners;

    private bool roundStarted = false;
    private Boundary boundaryController;
    private HUDController hudController;
    private int lastKilledCount = 0;

    void Awake() {
        boundaryController = boundary.GetComponent<Boundary>();
        hudController = canvas.GetComponent<HUDController>();
        exitButton.onClick.AddListener(ExitGame);
        startButton.onClick.AddListener(StartRound);
        hudController.UpdateArmor(Game.Instance.data.currentArmor);
        hudController.UpdateHealth(Game.Instance.data.currentHealth);
        hudController.UpdateRoundText();
    }

    void Update() {
        if (roundStarted) {
            Game.Instance.data.coins += (boundaryController.killedCount - lastKilledCount);
            hudController.UpdateArmor(Game.Instance.data.currentArmor);
            hudController.UpdateCoins(Game.Instance.data.coins);
            hudController.UpdateHealth(Game.Instance.data.currentHealth);
            lastKilledCount = boundaryController.killedCount;
            CheckIfGameOver();
        }
    }

    private void ExitGame() {
        Loader.Save();
        SceneManager.LoadScene("Start");
    }

    private void StartRound() {
        hudController.HideUpgradePanel();
        roundStarted = true;
        spawners.Spawn(Game.Instance.data.round++, roundTime);
    }

    private void ResetForNextRound() {
        hudController.ShowUpgradePanel();
        hudController.UpdateRoundText();
        hudController.UpdateHealth(Game.Instance.data.currentHealth);
        boundaryController.killedCount = 0;
        lastKilledCount = 0;
        roundStarted = false;
        Loader.Save();
    }

    private void GameIsOver() {
        Loader.Remove();
        Destroy(Game.Instance.gameObject.GetComponent<Game>());
        SceneManager.LoadScene("Start");
    }

    private void CheckIfGameOver() {
        if (Game.Instance.data.currentHealth <= 0) {
            gameOverPanel.SetActive(true);
            Invoke("GameIsOver", 3.0f);
        }

        if (lastKilledCount > 0 && spawners.transform.Find("Spawner").transform.childCount <= 0) {
            ResetForNextRound();
        }
    }
}

