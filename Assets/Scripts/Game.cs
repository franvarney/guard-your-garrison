using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class Game : MonoBehaviour {

    public PlayerData data = new PlayerData();

    private static Game instance;

    public static Game Instance {
        get {
            return instance;
        }
        set {
            instance = value;
        }
    }

    public void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public Game() {

    }
}

