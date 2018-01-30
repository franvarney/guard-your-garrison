using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {

    public GameObject[] enemies;
    public int startingEnemiesPerRound = 10;
    public GameObject spawner;

    private Coroutine coroutine;
    private float nextEnemies = 1;
    private int enemiesPerRound;

    private void Awake() {
        nextEnemies = startingEnemiesPerRound;
    }

    IEnumerator ISpawner(int enemiesAmount, float delay) {
        while (enemiesAmount-- > 0) {
            Vector3 position = spawner.transform.position;
            spawner.transform.position = new Vector3(spawner.transform.position.x, Random.Range(0f, 0.5f), spawner.transform.position.z);
            Instantiate(enemies[Random.Range(0, enemies.Length)], position, Quaternion.identity, spawner.transform);
            spawner.transform.position = position;
            yield return new WaitForSeconds(Random.Range(0, delay));
        }
    }

    private float GetRandomNumber(int round, float roundTime) {
        return Random.Range(Math.Min(round, roundTime), Math.Max(round, roundTime));
    }

    private float GetNextEnemiesAmount(int lastAmount, int round, float roundTime) {
        return enemiesPerRound * (1 + (GetRandomNumber(round, roundTime) / 100));
    }

    public void Spawn(int round, float roundTime) {
        roundTime = roundTime * (1 + (round / 100));
        enemiesPerRound = (int) nextEnemies;
        float delay = roundTime / enemiesPerRound;
        coroutine = StartCoroutine(ISpawner(enemiesPerRound, delay));
        nextEnemies = GetNextEnemiesAmount(enemiesPerRound, round, roundTime);
    }

    public void Stop() {
        StopCoroutine(coroutine);
    }
}
