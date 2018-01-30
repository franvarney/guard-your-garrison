using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour {

    public Button continueButton;
    public Button exitButton;
    public Button newGameButton;
    public Color disabledButtonColor;
    public GameObject game;

    void Awake() {
        //DontDestroyOnLoad(game);

        continueButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(ExitGame);
        newGameButton.onClick.AddListener(NewGame);

        Loader.Load();

        if (Loader.gameWasLoaded == false) {
            continueButton.interactable = false;
            continueButton.GetComponent<Image>().color = disabledButtonColor;
        }
    }

    void ContinueGame() {
        game.AddComponent<Game>();
        Game.Instance.data = Loader.savedGame;
        SceneManager.LoadScene("Game");
    }

    void ExitGame() {
        Application.Quit();
    }

    void NewGame() {
        Loader.Remove();
        Destroy(game.GetComponent<Game>());
        game.AddComponent<Game>();
        SceneManager.LoadScene("Game");
    }
}
