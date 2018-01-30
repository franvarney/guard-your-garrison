using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleController : MonoBehaviour {

    public int damage = 1;
    public int level = 1;
    public float speed = 1.0f;

    private EnemyController enemyController;

    /* TODO Castle attacks?
    
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy") {
            enemyController = other.gameObject.GetComponent<EnemyController>();
        }
    }

    IEnumerator IAttack(EnemyController enemy) {
        while (enemy != null || enemy.enabled || enemy.isActiveAndEnabled) {
            enemy.LoseHitPoints(damage);

            if (!enemy.enabled || !enemy.isActiveAndEnabled) {
                yield break;
            }

            yield return new WaitForSeconds(speed);
        }
    }

    private void Attack() {
        StartCoroutine(IAttack(enemyController));
    }*/
}
