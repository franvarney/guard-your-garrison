using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Loader : MonoBehaviour {

    public static bool gameWasLoaded = false;
    public static PlayerData savedGame;

    [DllImport("__Internal")] private static extern void SyncFiles();
    [DllImport("__Internal")] private static extern void WindowAlert(string message);

    private const string SAVED_GAME_FILE = "saved_game.save";

    private static string SavePath {
        get {
            return Path.Combine(Application.persistentDataPath, SAVED_GAME_FILE);
        }
    }

    private void Awake() {
        //Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
    }

    private static bool SaveExists(string path) {
        return File.Exists(path);
    }

    private static void PlatformSafeMessage(string message) {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            WindowAlert(message);
        } else {
            Debug.Log(message);
        }
    }

    public static void Save() {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream;

        savedGame = Game.Instance.data;

        try {
            fileStream = File.Create(Loader.SavePath);
            binaryFormatter.Serialize(fileStream, savedGame);
            fileStream.Close();

            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                SyncFiles();
            }
        } catch (Exception e) {
            PlatformSafeMessage("Failed to Save: " + e.Message);
        }
    }

    public static void Load() {
        try {
            if (File.Exists(Loader.SavePath)) {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(Loader.SavePath, FileMode.Open);

                savedGame = (PlayerData) binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                gameWasLoaded = true;
            }
        } catch (Exception e) {
            PlatformSafeMessage("Failed to Load: " + e.Message);
        }
    }

    public static bool Remove() {
        try {
            File.Delete(Loader.SavePath);
            gameWasLoaded = false;
        } catch (Exception) {
            return false;
        }

        return true;
    }
}
